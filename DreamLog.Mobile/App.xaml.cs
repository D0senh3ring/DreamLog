using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DreamLog.Views;
using DreamLog.Data;
using DreamLog.Data.Sql;
using Xamarin.Essentials;
using System.IO;
using System.Linq;
using DreamLog.Data.Models;
using DreamLog.DependencyInjection;
using System.Diagnostics;

namespace DreamLog
{
    public partial class App : Application
    {

        public App()
        {
            this.InitializeComponent();

#if DEBUG
            try
#endif
            {
                this.InitializeDatalayer();
            }
#if DEBUG
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
#endif

            this.MainPage = new MainPage();
        }

        private void InitializeDatalayer()
        {
            if (!DependencyContainer.IsRegistered<IDatalayer>())
            {
#if DEBUG
                string filename = "_debug_dreamlogs.sqlite";
#else
                string filename = "dreamlogs.sqlite";
#endif
                string filePath = Path.Combine(FileSystem.AppDataDirectory, filename);
                bool initialized = false;

                while (!initialized)
                {
                    this.CreateDatabaseFile(filePath);
                    this.InstantiateDatalayer(filePath);
                    initialized = this.DatalayerIsValid(DependencyContainer.GetSingleton<IDatalayer>());
                    if (!initialized)
                        this.DeleteDatabaseFile(filePath);
                }
            }
        }

        private void DeleteDatabaseFile(string filePath)
        {
            DependencyContainer.RemoveSingleton<IDatalayer>();
            FileInfo dataSourceFile = new FileInfo(filePath);
            if (dataSourceFile.Exists)
            {
                dataSourceFile.Delete();
            }
        }

        private void CreateDatabaseFile(string filePath)
        {
            FileInfo dataSourceFile = new FileInfo(filePath);
            if (!dataSourceFile.Exists)
            {
                dataSourceFile.Directory.Create();
                dataSourceFile.Create().Close();
            }
        }

        private void InstantiateDatalayer(string filePath)
        {
            try
            {
                IDatalayer datalayer = new DreamLogDatalayer(filePath);
                DependencyContainer.RegisterSingleton(datalayer);
                this.InitiallyPopulateDatalayer(datalayer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void InitiallyPopulateDatalayer(IDatalayer datalayer)
        {
            if (datalayer.DreamLogCollections.Count() == 0)
            {
                datalayer.AddDreamLog(new DreamLogCollection() { Name = "My dream collection" });
            }
        }

        /// <summary>
        /// Returns whether the sql-database could be read by EF-Core
        /// </summary>
        private bool DatalayerIsValid(IDatalayer datalayer)
        {
            try
            {
                datalayer.DreamLogCollections.Count();
                datalayer.DreamCategories.Count();
                datalayer.DreamLogEntries.Count();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}
