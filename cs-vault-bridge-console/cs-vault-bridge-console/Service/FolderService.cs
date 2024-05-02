using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Currency;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace cs_vault_bridge_console.Service
{
	internal class FolderService : BaseService
	{ 
		private List<File> files = new List<File>();
		public FolderService() { }
		public FolderService(string serverName, string vaultName, string userName, string password)
			:base(userName, password, serverName, vaultName) { 
		}
		private void CreateFolderAndAddFileTest()
		{

			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);

			foreach (EntityCategory category in connection.CategoryManager.GetAvailableCategories()) { 
				Console.WriteLine(category.Name);
				Console.WriteLine(category.ID);
			}

			//connection.FolderManager.CreateFolder(connection.FolderManager.RootFolder, "Creation_folder_test", false, connection.CategoryManager.FindCategory(2));
			FilePathAbsolute filePathAbsolute = new FilePathAbsolute("C:\\Users\\231223\\Documents\\GitHub\\cs-vault-bridge\\cs-vault-bridge-console\\cs-vault-bridge-console\\TestItem.txt");

			long fid;
			//VDF.Vault.Currency.Entities.Folder folder;
			foreach (VDF.Vault.Currency.Entities.Folder f in connection.FolderManager.GetChildFolders(connection.FolderManager.RootFolder,false, false)) {
				if (f.EntityName == "Creation_folder_test") {
					fid = f.EntityIterationId;
					connection.FileManager.AddFile(f, "Test comment", null, null, new FileClassification(), false, filePathAbsolute);
					break;
				}
			}
			base.Logout(connection);
		}
		private void PrintFilesInFolder(VDF.Vault.Currency.Entities.Folder parentFolder, VDF.Vault.Currency.Connections.Connection connection)
		{
			// get all the Files in the current Folder.
			File[] childFiles = connection.WebServiceManager.DocumentService.GetLatestFilesByFolderId(parentFolder.Id, false);

			//////Reflection///////				
			//MethodInfo mi = connection.WebServiceManager.DocumentService.GetType().GetTypeInfo().GetMethod("GetLatestFilesByFolderId");
			//File[] childFiles = (File[])mi.Invoke(connection.WebServiceManager.DocumentService, new object[] { parentFolder.Id, false });
			/////Update to spring boot
			Updater<File[]> updater1 = new Updater<File[]>("http://localhost:8080");
			updater1.GenericPost( updater1.baseUrl+"/post-files", childFiles);

			// print out any Files we find.
			if (childFiles != null && childFiles.Any())
			{
				foreach (File file in childFiles)
				{
					// print the full path, which is Folder name + File name.
					this.files.Add(file);
					//updater.GenericPost("post-file",file);
					Console.WriteLine(parentFolder.FullName + "/" + file.Name);
				}
			}

			// check for any sub Folders.
			IEnumerable<VDF.Vault.Currency.Entities.Folder> folders = connection.FolderManager.GetChildFolders(parentFolder, false, false);
			if (folders != null && folders.Any())
			{
				foreach (VDF.Vault.Currency.Entities.Folder folder in folders)
				{
					// recursively print the files in each sub-Folder
					PrintFilesInFolder(folder, connection);
				}
			}
		}
		public List<File> TestGetFolderStructure()
		{
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);
			try
			{
				// Start at the root Folder.
				VDF.Vault.Currency.Entities.Folder root = connection.FolderManager.RootFolder;
				// Call a function which prints all files in a Folder and sub-Folders.
				PrintFilesInFolder(root, connection);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString(), "Error");
				return null;
			}
			base.Logout(connection);
			return files;
		}
	}
}
