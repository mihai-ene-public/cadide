using IDE.Core;
using IDE.Core.Designers;
using IDE.Core.Storage;
using IDE.Core.ViewModels;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace IDE.Documents.Views
{
    public class NetClassesManagerViewModel : BaseViewModel
    {

        #region Commands

        ICommand addClassCommand;

        public ICommand AddClassCommand
        {
            get
            {
                if (addClassCommand == null)
                    addClassCommand = CreateCommand(p =>
                      {
                          var newClass = new NetClassDesignerItem { Id = LibraryItem.GetNextId(), Name = "New class" };
                          AddClass(CurrentClassNode as NetClassGroupDesignerItem, newClass);
                      });

                return addClassCommand;
            }
        }

        ICommand removeClassNodeCommand;

        //delete a group or a class
        public ICommand RemoveClassNodeCommand
        {
            get
            {
                if (removeClassNodeCommand == null)
                    removeClassNodeCommand = CreateCommand(p =>
                    {
                        RemoveClassNode(CurrentClassNode);
                    });

                return removeClassNodeCommand;
            }
        }

        ICommand addGroupCommand;

        public ICommand AddGroupCommand
        {
            get
            {
                if (addGroupCommand == null)
                    addGroupCommand = CreateCommand(p =>
                    {
                        var newGroup = new NetClassGroupDesignerItem { Id = LibraryItem.GetNextId(), Name = "New group" };
                        AddGroup(CurrentClassNode as NetClassGroupDesignerItem, newGroup);
                    });

                return addGroupCommand;
            }
        }

        ICommand addNetsToClassCommand;

        public ICommand AddNetsToClassCommand
        {
            get
            {
                if (addNetsToClassCommand == null)
                    addNetsToClassCommand = CreateCommand(p =>
                    {
                        var selectedNets = p as IList;
                        if (selectedNets == null)
                            return;
                        var currentClass = currentClassNode as NetClassDesignerItem;
                        if (currentClass == null)
                            return;

                        foreach (SchematicNet net in selectedNets)
                            net.ClassId = currentClass.Id;
                        RefreshNetsForClass();

                    },
                    p => currentClassNode as NetClassDesignerItem != null);

                return addNetsToClassCommand;
            }
        }

        ICommand removeNetsFromClassCommand;

        public ICommand RemoveNetsFromClassCommand
        {
            get
            {
                if (removeNetsFromClassCommand == null)
                    removeNetsFromClassCommand = CreateCommand(p =>
                    {
                        var selectedNets = p as IList;//<NetDesignerItem>;
                        if (selectedNets == null)
                            return;
                        //var currentClass = currentClassNode as NetClassDesignerItem;
                        //if (currentClass == null)
                        //    return;

                        foreach (SchematicNet net in selectedNets)
                            net.ClassId = 0;
                        RefreshNetsForClass();

                    },
                    p => currentClassNode as NetClassDesignerItem != null);

                return removeNetsFromClassCommand;
            }
        }

        #endregion

        public ObservableCollection<NetClassBaseDesignerItem> NetClasses { get; set; } = new ObservableCollection<NetClassBaseDesignerItem>();

        public ObservableCollection<SchematicNet> AvailableNets { get; set; } = new ObservableCollection<SchematicNet>();

        public ObservableCollection<SchematicNet> AssignedNets { get; set; } = new ObservableCollection<SchematicNet>();


        List<SchematicNet> allSchematicNets = new List<SchematicNet>();


        NetClassBaseDesignerItem currentClassNode;
        public NetClassBaseDesignerItem CurrentClassNode
        {
            get { return currentClassNode; }
            set
            {
                currentClassNode = value;
                OnPropertyChanged(nameof(CurrentClassNode));
                RefreshNetsForClass();
            }
        }

        void RefreshNetsForClass()
        {
            var currentClass = currentClassNode as NetClassDesignerItem;

            AvailableNets.Clear();
            AssignedNets.Clear();

            if (currentClass == null)
                return;

            AssignedNets.AddRange(allSchematicNets.Where(n => n.ClassId == currentClass.Id).OrderBy(n => n.Name));
            AvailableNets.AddRange(allSchematicNets.Where(n => n.ClassId == 0).Except(AssignedNets).OrderBy(n => n.Name));

        }

        void AddClass(NetClassGroupDesignerItem netGroup, NetClassDesignerItem netClass)
        {
            if (netGroup == null)
                NetClasses.Add(netClass);
            else
                netGroup.AddChild(netClass);
        }

        void RemoveClassNode(NetClassBaseDesignerItem netClass)
        {
            if (netClass.Parent != null)
            {
                netClass.Parent.RemoveChild(netClass);
            }
            else
            {
                NetClasses.Remove(netClass);
            }

        }

        void AddGroup(NetClassGroupDesignerItem parentGroup, NetClassGroupDesignerItem newGroup)
        {
            if (parentGroup != null)
                parentGroup.AddChild(newGroup);
            else
                NetClasses.Add(newGroup);
        }

        //void RemoveGroup(NetClassGroupDesignerItem netGroup)
        //{
        //    if (netGroup.Parent != null)
        //    {
        //        netGroup.Parent.RemoveChild(netGroup);
        //    }
        //    else
        //    {
        //        NetClasses.Remove(netGroup);
        //    }
        //}

        public void LoadFromSchematic(SchematicDocument schematic)
        {
            NetClasses.Clear();

            if (schematic == null || schematic.Classes == null)
                return;

            foreach (var docClass in schematic.Classes)
            {
                NetClasses.Add(NetClassBaseDesignerItem.LoadFrom(docClass));
            }
        }

        //loads net classes from current canvas (we could have added and removed net classes
        public void LoadFromCurrentSchematic(SchematicDesignerViewModel schematicModel)
        {
            allSchematicNets = (from sheet in schematicModel.Sheets
                                from lbl in sheet.Items.OfType<NetLabelCanvasItem>()
                                where lbl.Net != null && lbl.Net.IsNamed()
                                select lbl.Net).Distinct()
                           .OrderBy(p => p.Name)
                           .ToList();

            //AvailableNets.Clear();
            //AssignedNets.Clear();

            RefreshNetsForClass();
        }

        public void SaveToSchematic(SchematicDocument schematic)
        {
            schematic.Classes = NetClasses.Select(c => c.Save()).ToList();
        }
    }
}
