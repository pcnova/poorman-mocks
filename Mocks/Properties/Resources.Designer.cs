﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PoorMan.Mocks.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PoorMan.Mocks.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The behavior must be a delegate..
        /// </summary>
        internal static string InvalidBehaviorType {
            get {
                return ResourceManager.GetString("InvalidBehaviorType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The expression of type {0} does not identify a mockable member..
        /// </summary>
        internal static string InvalidMemberExpression {
            get {
                return ResourceManager.GetString("InvalidMemberExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The mock could not execute the custom behavior that was configured for the following operation: {0}. This is because the operation is being called with the wrong number or type of arguments. To fix this, make sure that:
        ///
        ///1) {1} is calling {2} with the same arguments passed to the operation, in the same order.
        ///
        ///2) the call that sets custom behavior for {1}, is passing a method or lambda with the same signature as {0}. {3}.
        /// </summary>
        internal static string MockBehaviorArgumentMismatchError {
            get {
                return ResourceManager.GetString("MockBehaviorArgumentMismatchError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A member-identifying expression must be provided..
        /// </summary>
        internal static string NoMemberExpression {
            get {
                return ResourceManager.GetString("NoMemberExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A value must be provided for the parameter and it must not be empty..
        /// </summary>
        internal static string NullOrEmptyArgument {
            get {
                return ResourceManager.GetString("NullOrEmptyArgument", resourceCulture);
            }
        }
    }
}
