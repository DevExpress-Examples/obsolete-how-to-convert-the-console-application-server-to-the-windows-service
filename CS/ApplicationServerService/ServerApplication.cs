using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using SecuritySystemExample.Module;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Web.SystemModule;

namespace ApplicationServerService {
    public class ApplicationServerServiceServerApplication : ServerApplication {
        public ApplicationServerServiceServerApplication() {
            this.ApplicationName = "SecuritySystemExample";
            this.Modules.Add(new SystemWindowsFormsModule());
            this.Modules.Add(new SystemAspNetModule());
            this.Modules.Add(new SecuritySystemExampleModule());
            this.Modules.Add(new SecurityModule());
        }
        protected override void OnDatabaseVersionMismatch(DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs args) {
            args.Updater.Update();
            args.Handled = true;
        }
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);
        }
    }
}
