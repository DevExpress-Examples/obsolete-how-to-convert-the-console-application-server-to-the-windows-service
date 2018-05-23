Imports System
Imports System.Configuration
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Diagnostics
Imports System.ServiceProcess
Imports System.Text
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Security
Imports DevExpress.Persistent.Base
Imports DevExpress.ExpressApp.Security.ClientServer
Imports System.Runtime.Remoting
Imports DevExpress.ExpressApp.Security.ClientServer.Remoting
Imports DevExpress.ExpressApp.Xpo
Imports DevExpress.ExpressApp.MiddleTier
Imports System.ServiceModel
Imports DevExpress.ExpressApp.Security.Strategy
Imports DevExpress.ExpressApp.Security.ClientServer.Wcf

Namespace ApplicationServerService
    Partial Public Class ApplicationServerService
        Inherits System.ServiceProcess.ServiceBase

        Private Sub serverApplication_DatabaseVersionMismatch(ByVal sender As Object, ByVal e As DatabaseVersionMismatchEventArgs)
            e.Updater.Update()
            e.Handled = True
        End Sub
        Private serviceHost As ServiceHost
        Protected Overrides Sub OnStart(ByVal args() As String)
            ValueManager.ValueManagerType = GetType(MultiThreadValueManager(Of )).GetGenericTypeDefinition()
            'string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            InMemoryDataStoreProvider.Register()
            Dim connectionString As String = InMemoryDataStoreProvider.ConnectionString
            Dim serverApplication As New ApplicationServerServiceServerApplication()
            serverApplication.ConnectionString = connectionString
            serverApplication.Setup()
            serverApplication.CheckCompatibility()
            serverApplication.Dispose()
            Dim securityProviderHandler As QueryRequestSecurityStrategyHandler = Function() New SecurityStrategyComplex(GetType(SecuritySystemUser), GetType(SecuritySystemRole), New AuthenticationStandard())
            Dim dataServer As New SecuredDataServer(connectionString, XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary, securityProviderHandler)
            serviceHost = New ServiceHost(New WcfSecuredDataServer(dataServer))
            serviceHost.AddServiceEndpoint(GetType(IWcfSecuredDataServer), WcfDataServerHelper.CreateDefaultBinding(), "http://localhost:1451/DataServer1")
            serviceHost.Open()
        End Sub
        Protected Overrides Sub OnStop()
            serviceHost.Close()
        End Sub
        Public Sub New()
            InitializeComponent()
        End Sub
    End Class
End Namespace
