using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security.ClientServer;
using System.Runtime.Remoting;
using DevExpress.ExpressApp.Security.ClientServer.Remoting;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.MiddleTier;
using System.ServiceModel;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Security.ClientServer.Wcf;

namespace ApplicationServerService {
    public partial class ApplicationServerService : System.ServiceProcess.ServiceBase {
        private void serverApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
            e.Updater.Update();
            e.Handled = true;
        }
        private ServiceHost serviceHost;
        protected override void OnStart(string[] args) {
            ValueManager.ValueManagerType = typeof(MultiThreadValueManager<>).GetGenericTypeDefinition();
            //string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            InMemoryDataStoreProvider.Register();
            string connectionString = InMemoryDataStoreProvider.ConnectionString;
            ApplicationServerServiceServerApplication serverApplication = 
                new ApplicationServerServiceServerApplication();
            serverApplication.ConnectionString = connectionString;
            serverApplication.Setup();
            serverApplication.CheckCompatibility();
            serverApplication.Dispose();
            QueryRequestSecurityStrategyHandler securityProviderHandler = delegate() {
                return new SecurityStrategyComplex(typeof(SecuritySystemUser), 
                    typeof(SecuritySystemRole), new AuthenticationStandard());
            };
            SecuredDataServer dataServer = new SecuredDataServer(
                connectionString, XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary,
                        securityProviderHandler);
            serviceHost = new ServiceHost(new WcfSecuredDataServer(dataServer));
            serviceHost.AddServiceEndpoint(typeof(IWcfSecuredDataServer),
                WcfDataServerHelper.CreateDefaultBinding(),
                "http://localhost:1451/DataServer");
            serviceHost.Open();
        }
        protected override void OnStop() {
            serviceHost.Close();
        }
        public ApplicationServerService() {
            InitializeComponent();
        }
    }
}
