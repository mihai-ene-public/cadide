using System;
using System.Windows.Input;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.Resources;
using IDE.Core.Types.Input;

namespace IDE.Core.Commands
{

    public class AppCommand
    {
        #region CommandFramework Fields

        static ICommand about;
        static ICommand programSettings;
        static ICommand showToolWindow;

        static ICommand loadFile;
        static ICommand saveAll;
        static ICommand exportUMLToImage;
        static ICommand exportTextToHTML;

        static ICommand closeFile;
        static ICommand viewTheme;

        static ICommand browseURL;
        static ICommand showStartPage;


        #region Text Edit Commands

        static ICommandFactory commandFactory;

        #endregion Text Edit Commands
        #endregion CommandFramework Fields

        #region Static Constructor (Constructs static application commands)
        /// <summary>
        /// Define custom commands and their key gestures
        /// </summary>
        static AppCommand()
        {
            commandFactory = ServiceProvider.Resolve<ICommandFactory>();


            //InputGestureCollection inputs = null;

            // Initialize the exit command
            //inputs = new InputGestureCollection();
            //inputs.Add(new KeyGesture(Key.F4, ModifierKeys.Alt, "Alt+F4"));
            //exit = CreateUICommand(Strings.CMD_App_Exit_Describtion, "Exit", typeof(AppCommand), XKey.F4, XModifierKeys.Alt);

            about = CreateUICommand(Strings.CMD_APP_About_Description, "About", typeof(AppCommand));

            programSettings = CreateUICommand("Edit or Review your program settings", "ProgramSettings", typeof(AppCommand));

            showToolWindow = CreateUICommand("Hide or display toolwindow", "ShowToolWindow", typeof(AppCommand));

            // Execute file open command (without user interaction)
            loadFile = CreateUICommand(Strings.CMD_APP_Open_Description, "LoadFile", typeof(AppCommand));

            saveAll = CreateUICommand(Strings.CMD_APP_SaveAll_Description, "SaveAll", typeof(AppCommand));

            exportUMLToImage = CreateUICommand(Strings.CMD_APP_ExportUMLToImage_Description, "ExportUMLToImage", typeof(AppCommand));

            exportTextToHTML = CreateUICommand(Strings.CMD_APP_ExportTextToHTML_Description, "ExportTextToHTML", typeof(AppCommand));

            closeFile = CreateUICommand(Strings.CMD_APP_CloseDoc_Description, "Close", typeof(AppCommand), XKey.F4, XModifierKeys.Control);

            // Initialize the viewTheme command
            viewTheme = CreateUICommand(Strings.CMD_APP_ViewTheme_Description, "ViewTheme", typeof(AppCommand));

            // Execute browse Internt URL (without user interaction)
            browseURL = CreateUICommand(Strings.CMD_APP_OpenURL_Description, "OpenURL", typeof(AppCommand));

            showStartPage = CreateUICommand(Strings.CMD_APP_ShowStartPage_Description, "StartPage", typeof(AppCommand));

        }
        #endregion Static Constructor

        #region CommandFramework Properties (Exposes Commands to which the UI can bind to)

        static ICommand exit;

        /// <summary>
        /// Static property of the correspondong <seealso cref="System.Windows.Input.ICommand"/>
        /// </summary>
        public static ICommand Exit
        {
            get
            {
                if (exit == null)
                    exit = CreateUICommand();

                return exit;
            }
        }

        public static ICommand About
        {
            get { return about; }
        }


        public static ICommand ProgramSettings
        {
            get
            {
                return programSettings;
            }
        }

        public static ICommand ShowToolWindow
        {
            get
            {
                return showToolWindow;
            }
        }

        /// <summary>
        /// Execute file open command (without user interaction)
        /// </summary>
        public static ICommand LoadFile
        {
            get { return loadFile; }
        }

        /// <summary>
        /// Execute a command to save all edited files and current program settings
        /// </summary>
        public static ICommand SaveAll
        {
            get { return saveAll; }
        }

        /// <summary>
        /// Execute a command to export the currently loaded UML Diagram (XML based data)
        /// into an image based data format (png, jpeg, wmf)
        /// </summary>
        public static ICommand ExportUMLToImage
        {
            get { return exportUMLToImage; }
        }

        /// <summary>
        /// Execute a command to export the currently loaded and highlighted text (XML, C# ...)
        /// into an HTML data format (*.htm, *.html ...)
        /// </summary>
        public static ICommand ExportTextToHTML
        {
            get { return exportTextToHTML; }
        }

        public static ICommand CloseFile
        {
            get { return closeFile; }
        }

        static ICommand closeSolution;
        public static ICommand CloseSolution
        {
            get
            {
                if (closeSolution == null)
                    closeSolution = CreateUICommand("Close solution", "CloseSolution", typeof(AppCommand));

                return closeSolution;
            }
        }

        /// <summary>
        /// Static property of the correspondong <seealso cref="System.Windows.Input.ICommand"/>
        /// </summary>
        public static ICommand ViewTheme
        {
            get { return viewTheme; }
        }

        /// <summary>
        /// Browse to an Internet URL via default web browser configured in Windows
        /// </summary>
        public static ICommand BrowseURL
        {
            get { return browseURL; }
        }

        /// <summary>
        /// Static property of the correspondong <seealso cref="System.Windows.Input.ICommand"/>
        /// </summary>
        public static ICommand ShowStartPage
        {
            get { return showStartPage; }
        }

        #region Solution Commands

        static ICommand buildCommand;

        /// <summary>
        /// Build can be applied to both a Solution and a Project
        /// </summary>
        public static ICommand BuildCommand
        {
            get
            {
                if (buildCommand == null)
                    buildCommand = CreateUICommand("Build", "Build", typeof(AppCommand));

                return buildCommand;
            }
        }

        static ICommand compileCommand;

        /// <summary>
        /// Compile can be applied to both a Solution and a Project
        /// </summary>
        public static ICommand CompileCommand
        {
            get
            {
                if (compileCommand == null)
                    compileCommand = CreateUICommand("Compile", "Compile", typeof(AppCommand));

                return compileCommand;
            }
        }

        static ICommand packCommand;

        /// <summary>
        /// Create package from both a Solution or a Project
        /// </summary>
        public static ICommand PackCommand
        {
            get
            {
                if (packCommand == null)
                    packCommand = CreateUICommand("Pack", "Pack", typeof(AppCommand));

                return packCommand;
            }
        }

        static ICommand restorePackagesCommand;

        /// <summary>
        /// Create package from both a Solution or a Project
        /// </summary>
        public static ICommand RestorePackagesCommand
        {
            get
            {
                if (restorePackagesCommand == null)
                    restorePackagesCommand = CreateUICommand("RestorePacks", "RestorePacks", typeof(AppCommand));

                return restorePackagesCommand;
            }
        }

        static ICommand addProjectCommand;

        /// <summary>
        /// Add a new project to the current solution
        /// </summary>
        public static ICommand AddProjectCommand
        {
            get
            {
                if (addProjectCommand == null)
                {
                    addProjectCommand = CreateUICommand();
                }

                return addProjectCommand;
            }
        }

        static ICommand manageReferencesCommand;
        public static ICommand ManageReferencesCommand
        {
            get
            {
                if (manageReferencesCommand == null)
                {
                    manageReferencesCommand = CreateUICommand();
                }

                return manageReferencesCommand;
            }
        }

        static ICommand showPackageManagerCommand;
        public static ICommand ShowPackageManagerCommand
        {
            get
            {
                if (showPackageManagerCommand == null)
                {
                    showPackageManagerCommand = CreateUICommand();
                }

                return showPackageManagerCommand;
            }
        }

        static ICommand addGroupFolder;

        /// <summary>
        /// Add a new group folder to the solution. Projects can be grouped inside these folders
        /// </summary>
        public static ICommand AddGroupFolderCommand
        {
            get
            {
                if (addGroupFolder == null)
                    addGroupFolder = CreateUICommand("AddGroupFolder", "AddGroupFolder", typeof(AppCommand));

                return addGroupFolder;
            }
        }

        static ICommand addFolderCommand;

        /// <summary>
        /// Add a real Directory on the disk. This can be added to the solution path or inside any path of a project
        /// <para>It is used to group real files on the disk</para>
        /// </summary>
        public static ICommand AddFolderCommand
        {
            get
            {
                if (addFolderCommand == null)
                    addFolderCommand = CreateUICommand("AddFolder", "AddFolder", typeof(AppCommand));

                return addFolderCommand;
            }
        }

        static ICommand openFolderInFileExplorerCommand;

        /// <summary>
        /// Command can be applied on any selected item (file/folder/project/solution) in SolutionExplorer except virtual folders
        /// </summary>
        public static ICommand OpenFolderInFileExplorerCommand
        {
            get
            {
                if (openFolderInFileExplorerCommand == null)
                    openFolderInFileExplorerCommand = CreateUICommand("OpenFolderFileExplorer", "OpenFolderFileExplorer", typeof(AppCommand));

                return openFolderInFileExplorerCommand;
            }
        }

        static ICommand openConfigurationManagerCommand;

        /// <summary>
        /// Command can be applied for Solution Item only
        /// </summary>
        public static ICommand OpenConfigurationManagerCommand
        {
            get
            {
                if (openConfigurationManagerCommand == null)
                    openConfigurationManagerCommand = CreateUICommand("OpenConfigurationManager", "OpenConfigurationManager", typeof(AppCommand));

                return openConfigurationManagerCommand;
            }
        }

        static ICommand openProjectDependenciesCommand;

        /// <summary>
        /// Command can be applied for Solution Item only
        /// </summary>
        public static ICommand OpenProjectDependenciesCommand
        {
            get
            {
                if (openProjectDependenciesCommand == null)
                    openProjectDependenciesCommand = CreateUICommand("OpenProjectDependencies", "OpenProjectDependencies", typeof(AppCommand));

                return openProjectDependenciesCommand;
            }
        }


        static ICommand addNewItemCommand;

        /// <summary>
        /// For Solution: Misc files; For Project: Misc + item depending of project type (Library/Gerber)
        /// </summary>
        public static ICommand AddNewItemCommand
        {
            get
            {
                if (addNewItemCommand == null)
                    addNewItemCommand = CreateUICommand("NewItem", "NewItem", typeof(AppCommand));

                return addNewItemCommand;
            }
        }

        static ICommand addExistingItemCommand;

        /// <summary>
        /// For Solution: Misc files; For Project: Misc + item depending of project type (Library/Gerber)
        /// </summary>
        public static ICommand AddExistingItemCommand
        {
            get
            {
                if (addExistingItemCommand == null)
                    addExistingItemCommand = CreateUICommand("AddExistingItem", "AddExistingItem", typeof(AppCommand));

                return addExistingItemCommand;
            }
        }


        #endregion Solution Commands

        #region Project Commands

        // static ICommand addNewFolderCommand;



        static ICommand addNewSymbolCommand;

        public static ICommand AddNewSymbolCommand
        {
            get
            {
                if (addNewSymbolCommand == null)
                    addNewSymbolCommand = CreateUICommand(" AddNewSymbol", " AddNewSymbol", typeof(AppCommand));

                return addNewSymbolCommand;
            }
        }

        static ICommand addNewFootprintCommand;

        public static ICommand AddNewFootprintCommand
        {
            get
            {
                if (addNewFootprintCommand == null)
                    addNewFootprintCommand = CreateUICommand(" AddNewFootprint", " AddNewFootprint", typeof(AppCommand));

                return addNewFootprintCommand;
            }
        }



        static ICommand addNewModelCommand;

        public static ICommand AddNewModelCommand
        {
            get
            {
                if (addNewModelCommand == null)
                    addNewModelCommand = CreateUICommand(" AddNewModel", " AddNewModel", typeof(AppCommand));

                return addNewModelCommand;
            }
        }

        static ICommand addNewComponentCommand;

        public static ICommand AddNewComponentCommand
        {
            get
            {
                if (addNewComponentCommand == null)
                    addNewComponentCommand = CreateUICommand(" AddNewComponent", " AddNewComponent", typeof(AppCommand));

                return addNewComponentCommand;
            }
        }

        static ICommand addNewBoardCommand;

        public static ICommand AddNewBoardCommand
        {
            get
            {
                if (addNewBoardCommand == null)
                    addNewBoardCommand = CreateUICommand(" AddNewBoard", " AddNewBoard", typeof(AppCommand));

                return addNewBoardCommand;
            }
        }

        static ICommand addNewSchematicCommand;

        public static ICommand AddNewSchematicCommand
        {
            get
            {
                if (addNewSchematicCommand == null)
                    addNewSchematicCommand = CreateUICommand(" AddNewSchematic", " AddNewSchematic", typeof(AppCommand));

                return addNewSchematicCommand;
            }
        }

        static ICommand addNewDiagramCommand;

        public static ICommand AddNewDiagramCommand
        {
            get
            {
                if (addNewDiagramCommand == null)
                    addNewDiagramCommand = CreateUICommand(" AddNewDiagram", " AddNewDiagram", typeof(AppCommand));

                return addNewDiagramCommand;
            }
        }

        static ICommand openItemCommand;
        public static ICommand OpenItemCommand
        {
            get
            {
                if (openItemCommand == null)
                    openItemCommand = CreateUICommand();

                return openItemCommand;
            }
        }

        static ICommand showPropertiesCommand;
        public static ICommand ShowPropertiesCommand
        {
            get
            {
                if (showPropertiesCommand == null)
                    showPropertiesCommand = CreateUICommand();

                return showPropertiesCommand;
            }
        }

        #endregion Project Commands

        static ICommand importEagleCommand;
        public static ICommand ImportEagleCommand
        {
            get
            {
                if (importEagleCommand == null)
                    importEagleCommand = CreateUICommand();

                return importEagleCommand;
            }
        }

        static ICommand changeModeCommand;

        /// <summary>
        /// TAB
        /// </summary>
        public static ICommand ChangeModeCommand
        {
            get
            {
                if (changeModeCommand == null)
                {
                    changeModeCommand = CreateUICommand("Change Mode", "changeMode", typeof(AppCommand), XKey.Tab);
                }

                return changeModeCommand;
            }
        }

        static ICommand cyclePlacementOrRotateCommand;

        /// <summary>
        /// SPACE
        /// </summary>
        public static ICommand CyclePlacementOrRotateCommand
        {
            get
            {
                if (cyclePlacementOrRotateCommand == null)
                {
                    cyclePlacementOrRotateCommand = CreateUICommand("CyclePlacementOrRotate", "CyclePlacementOrRotate", typeof(AppCommand), XKey.Space);
                }

                return cyclePlacementOrRotateCommand;
            }
        }

        static ICommand copySelectedItemsCommand;
        public static ICommand CopySelectedItemsCommand
        {
            get
            {
                if (copySelectedItemsCommand == null)
                {
                    copySelectedItemsCommand = CreateUICommand("copySelectedItemsCommand", "copySelectedItemsCommand", typeof(AppCommand), XKey.C, XModifierKeys.Control);
                }

                return copySelectedItemsCommand;
            }
        }


        static ICommand pasteSelectedItemsCommand;
        public static ICommand PasteSelectedItemsCommand
        {
            get
            {
                if (pasteSelectedItemsCommand == null)
                {
                    pasteSelectedItemsCommand = CreateUICommand("pasteSelectedItemsCommand", "pasteSelectedItemsCommand", typeof(AppCommand), XKey.V, XModifierKeys.Control);
                }

                return pasteSelectedItemsCommand;
            }
        }

        static ICommand deleteSelectedItemsCommand;
        public static ICommand DeleteSelectedItemsCommand
        {
            get
            {
                if (deleteSelectedItemsCommand == null)
                {
                    deleteSelectedItemsCommand = CreateUICommand("deleteSelectedItemsCommand", "deleteSelectedItemsCommand", typeof(AppCommand), XKey.Delete);
                }

                return deleteSelectedItemsCommand;
            }
        }

        static ICommand newCommand;

        public static ICommand New
        {
            get
            {
                if (newCommand == null)
                    newCommand = CreateUICommand();

                return newCommand;
            }
        }


        static ICommand openCommand;
        public static ICommand Open
        {
            get
            {
                if (openCommand == null)
                    openCommand = CreateUICommand();

                return openCommand;
            }
        }

        static ICommand saveCommand;
        public static ICommand Save
        {
            get
            {
                if (saveCommand == null)
                    saveCommand = CreateUICommand("Save", "Save", typeof(AppCommand), XKey.S, XModifierKeys.Control);

                return saveCommand;
            }
        }

        static ICommand saveAsCommand;

        public static ICommand SaveAs
        {
            get
            {
                if (saveAsCommand == null)
                    saveAsCommand = CreateUICommand();

                return saveAsCommand;
            }
        }
        #endregion CommandFramwork_Properties

        static ICommand CreateUICommand(string text, string name, Type ownerType)
        {
            return commandFactory.CreateUICommand(text, name, ownerType);
        }

        static ICommand CreateUICommand(string text, string name)
        {
            return commandFactory.CreateUICommand(text, name, typeof(AppCommand));
        }

        static ICommand CreateUICommand(string text, string name, Type ownerType, XKey key, XModifierKeys modifierKeys = XModifierKeys.None)
        {
            return commandFactory.CreateUICommand(text, name, ownerType, key, modifierKeys);
        }

        static ICommand CreateUICommand(string text, string name, Type ownerType, XKey key)
        {
            return commandFactory.CreateUICommand(text, name, ownerType, key);
        }

        static ICommand CreateUICommand()
        {
            return commandFactory.CreateUICommand();
        }
    }
}
