﻿#pragma checksum "..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "DF8E44A988533D9C1D7C1CD9690D5B14928074713D9EA5E9B44546B4DC2EE46E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Soxkets;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Soxkets {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 31 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Frame Main;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle CloseBBG;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image CloseButton;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle MaximizeBBG;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image MaximizeButton;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle MinimizeBBG;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image MinimizeButton;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image InterfaceToggle;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Soxkets;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Main = ((System.Windows.Controls.Frame)(target));
            return;
            case 2:
            
            #line 34 "..\..\MainWindow.xaml"
            ((System.Windows.Shapes.Rectangle)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Rectangle_MouseDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.CloseBBG = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 36 "..\..\MainWindow.xaml"
            this.CloseBBG.MouseLeave += new System.Windows.Input.MouseEventHandler(this.CloseBBG_MouseLeave);
            
            #line default
            #line hidden
            
            #line 36 "..\..\MainWindow.xaml"
            this.CloseBBG.MouseEnter += new System.Windows.Input.MouseEventHandler(this.CloseBBG_MouseEnter);
            
            #line default
            #line hidden
            return;
            case 4:
            this.CloseButton = ((System.Windows.Controls.Image)(target));
            
            #line 37 "..\..\MainWindow.xaml"
            this.CloseButton.MouseLeave += new System.Windows.Input.MouseEventHandler(this.CloseBBG_MouseLeave);
            
            #line default
            #line hidden
            
            #line 37 "..\..\MainWindow.xaml"
            this.CloseButton.MouseEnter += new System.Windows.Input.MouseEventHandler(this.CloseBBG_MouseEnter);
            
            #line default
            #line hidden
            
            #line 37 "..\..\MainWindow.xaml"
            this.CloseButton.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.CloseButton_MouseUp);
            
            #line default
            #line hidden
            return;
            case 5:
            this.MaximizeBBG = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 39 "..\..\MainWindow.xaml"
            this.MaximizeBBG.MouseEnter += new System.Windows.Input.MouseEventHandler(this.MaximizeBBG_MouseEnter);
            
            #line default
            #line hidden
            
            #line 39 "..\..\MainWindow.xaml"
            this.MaximizeBBG.MouseLeave += new System.Windows.Input.MouseEventHandler(this.MaximizeBBG_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 6:
            this.MaximizeButton = ((System.Windows.Controls.Image)(target));
            
            #line 40 "..\..\MainWindow.xaml"
            this.MaximizeButton.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.MaximizeButton_MouseUp);
            
            #line default
            #line hidden
            
            #line 40 "..\..\MainWindow.xaml"
            this.MaximizeButton.MouseEnter += new System.Windows.Input.MouseEventHandler(this.MaximizeBBG_MouseEnter);
            
            #line default
            #line hidden
            
            #line 40 "..\..\MainWindow.xaml"
            this.MaximizeButton.MouseLeave += new System.Windows.Input.MouseEventHandler(this.MaximizeBBG_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 7:
            this.MinimizeBBG = ((System.Windows.Shapes.Rectangle)(target));
            
            #line 42 "..\..\MainWindow.xaml"
            this.MinimizeBBG.MouseEnter += new System.Windows.Input.MouseEventHandler(this.MinimizeBBG_MouseEnter);
            
            #line default
            #line hidden
            
            #line 42 "..\..\MainWindow.xaml"
            this.MinimizeBBG.MouseLeave += new System.Windows.Input.MouseEventHandler(this.MinimizeBBG_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 8:
            this.MinimizeButton = ((System.Windows.Controls.Image)(target));
            
            #line 43 "..\..\MainWindow.xaml"
            this.MinimizeButton.MouseEnter += new System.Windows.Input.MouseEventHandler(this.MinimizeBBG_MouseEnter);
            
            #line default
            #line hidden
            
            #line 43 "..\..\MainWindow.xaml"
            this.MinimizeButton.MouseLeave += new System.Windows.Input.MouseEventHandler(this.MinimizeBBG_MouseLeave);
            
            #line default
            #line hidden
            
            #line 43 "..\..\MainWindow.xaml"
            this.MinimizeButton.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.MinimizeButton_MouseUp);
            
            #line default
            #line hidden
            return;
            case 9:
            this.InterfaceToggle = ((System.Windows.Controls.Image)(target));
            
            #line 46 "..\..\MainWindow.xaml"
            this.InterfaceToggle.MouseUp += new System.Windows.Input.MouseButtonEventHandler(this.InterfaceToggle_MouseUp);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 47 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.TextBlock)(target)).MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Rectangle_MouseDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

