Imports System.ServiceProcess
Imports System.Text

Namespace ApplicationServerService
    Friend NotInheritable Class Program

        Private Sub New()
        End Sub

        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        Shared Sub Main()
            Dim ServicesToRun() As ServiceBase
            ServicesToRun = New ServiceBase() { New ApplicationServerService() }
            ServiceBase.Run(ServicesToRun)
        End Sub
    End Class
End Namespace
