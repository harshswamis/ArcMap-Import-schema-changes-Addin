using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ArcMap_ImportSchemaChangesAddin
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class ArcMap_ImportSchemaChangesAddinPanel : UserControl
    {
        public ArcMap_ImportSchemaChangesAddinPanel(object hook)
        {
            InitializeComponent();
            this.Hook = hook;
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }


        ISchemaChanges schemaChanges;
        IName name;

        string targetGeodatabase;
        string schemaFile;

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private ArcMap_ImportSchemaChangesAddinPanel m_windowUI;

            public AddinImpl()
            {

                
            }

            

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new ArcMap_ImportSchemaChangesAddinPanel(this.Hook);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            schemaFile = textBox1.Text;
            targetGeodatabase = textBox2.Text;

            IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactory();

            IWorkspace targetWorkspace = workspaceFactory.OpenFromFile(targetGeodatabase, 0);

            ListSchemaChanges(targetWorkspace, schemaFile, true);
            MessageBox.Show("Successfully Updated!");
        }

        public void ListSchemaChanges(IWorkspace targetWorkspace, String schemaFile, Boolean
   isSchemaChangeFile)
        {
            // Workspace name object for the target.
            IDataset dataset = (IDataset)targetWorkspace;
            IWorkspaceName workspaceName = (IWorkspaceName)dataset.FullName;

            string path = workspaceName.PathName;

            // Initialize the schema changes object.
            ISchemaChangesInit schemaChangesInit = (ISchemaChangesInit)new SchemaChanges();
            if (isSchemaChangeFile)
            {
                schemaChangesInit.InitFromSchemaDifferencesDocument(schemaFile,
                   workspaceName);
            }
            else
            {
                schemaChangesInit.InitFromSchemaDocument(schemaFile, workspaceName);
            }
            schemaChanges = (ISchemaChanges)schemaChangesInit;

            // Print the changes.
            PrintSchemaChanges(schemaChanges.GetChanges(), "");

            IImportSchema importSchema = new ReplicaSchemaImporter();

            importSchema.ImportSchema(schemaChanges);


        }

        public void PrintSchemaChanges(IEnumSchemaChange enumSchemaChanges, String
    schemaChange)
        {
            enumSchemaChanges.Reset();
            ISchemaChangeInfo schemaChangesInfo = null;
            String dataName = "";

            // Step through the schema changes.
            while ((schemaChangesInfo = enumSchemaChanges.Next()) != null)
            {
                // Get the name of the feature class, feature dataset, and so on, that has changed.
                if (schemaChangesInfo.SchemaChangeType !=
                    esriSchemaChangeType.esriSchemaChangeTypeNoChange)
                {
                    if (schemaChangesInfo.ToObject != null)
                    {
                        dataName = GetDataName(schemaChangesInfo.ToObject);

                    }
                    else if (schemaChangesInfo.FromObject != null)
                    {
                        dataName = GetDataName(schemaChangesInfo.FromObject);

                    }
                }
                schemaChangesInfo.ApplySchemaChange = true;
                // If at the end of the list for the schema change, print the schema change. 
                // Otherwise, continue through the list.
                if (schemaChangesInfo.GetChanges() != null)
                {
                    String nextSchemaChange = String.Format("{0}{1}/", schemaChange,
                        dataName);
                    PrintSchemaChanges(schemaChangesInfo.GetChanges(), nextSchemaChange);
                    schemaChangesInfo.ApplySchemaChange = true;
                    return;
                }
                else
                {
                    // Convert the schema change type to a string and print the change.
                    String changeType = Enum.GetName(typeof(esriSchemaChangeType),
                        schemaChangesInfo.SchemaChangeType);
                    //Console.WriteLine("{0}{1}: {2}", schemaChange, dataName, changeType);
                    //MessageBox.Show("{0}{1}: {2}" + schemaChange + dataName + changeType);
                    schemaChangesInfo.ApplySchemaChange = true;
                    
                }
                
            }
        }

        public String GetDataName(object data)
        {
            if (data is IDataElement)
            {
                IDataElement dataElement = (IDataElement)data;
                return dataElement.Name;
            }
            else if (data is IFields)
            {
                return "Field Changes";
            }
            else if (data is IField)
            {
                IField field = (IField)data;
                return field.Name;
            }
            else if (data is IDomain)
            {
                IDomain domain = (IDomain)data;
                return domain.Name;
            }
            else
            {
                return "Unknown type";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            textBox1.Clear();
            textBox2.Clear();

            UID dockWinID = new UIDClass();
            dockWinID.Value = ThisAddIn.IDs.ArcMap_ImportSchemaChangesAddinPanel;

            // Use GetDockableWindow directly as we want the client IDockableWindow not the internal class  
            IDockableWindow dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
            dockWindow.Show(false);
        }

        private void ArcMap_ImportSchemaChangesAddinPanel_VisibleChanged(object sender, EventArgs e)
        {

            //UID dockWinID = new UIDClass();
            //dockWinID.Value = ThisAddIn.IDs.ArcMap_ImportSchemaChangesAddinPanel;

            //// Use GetDockableWindow directly as we want the client IDockableWindow not the internal class  
            //IDockableWindow dockWindow = ArcMap.DockableWindowManager.GetDockableWindow(dockWinID);
            //dockWindow.Show(false);
            if (base.Visible)
            { MessageBox.Show("Clicked on X"); }
        }
    }
}
