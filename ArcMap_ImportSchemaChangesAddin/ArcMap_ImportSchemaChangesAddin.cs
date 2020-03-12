using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

namespace ArcMap_ImportSchemaChangesAddin
{
    public class ArcMap_ImportSchemaChangesAddin : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public ArcMap_ImportSchemaChangesAddin()
        {
        }

        

        protected override void OnClick()
        {
            UID dockWinID = new UIDClass();
            dockWinID.Value = ThisAddIn.IDs.ArcMap_ImportSchemaChangesAddinPanel;

            // Use GetDockableWindow directly as we want the client IDockableWindow not the internal class  
            IDockableWindow dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
            dockWindow.Show(true);
            
            
            ArcMap.Application.CurrentTool = null;
        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
