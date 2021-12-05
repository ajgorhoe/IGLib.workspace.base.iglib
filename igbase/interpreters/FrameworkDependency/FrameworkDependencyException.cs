using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace IG.Lib
{

    /// <summary>Exception that should be thrown by IGLib libraries and applications when the reason for an error is dependency on 
    /// missing (not implemeted) APIs from the .NET full frameworrk while the code is build against .NET Core, .NET 5 or later, or
    /// some other framework that also des not support those APIS.</summary>
    public class FrameworkDependencyException: NotImplementedException
    {

        /// <summary>Initializes a new instance of the <see cref="FrameworkDependencyException"/> class with the speecified error message.</summary>
        /// <param name="message">The message that describes the error. Standard part will be added to the message explaining that the error was caused by
        /// dependency on the /NET Framework (full legacy framework) while the code targets the .NET Core framework or other. This is done by 
        /// transforming the <paramref name="message"/> by <see cref="TransformErrorMessage(string)"/>.</param>
        /// <seealso cref="TransformErrorMessage(string)"/>
        public FrameworkDependencyException(string message): base(TransformErrorMessage(message))
        {  }

        /// <summary>Initializes a new instance of the <see cref="FrameworkDependencyException"/> class with the speecified error message
        /// and inner exception.</summary>
        /// <param name="message">The message that describes the error. Standard part will be added to the message explaining that the error was caused by
        /// dependency on the /NET Framework (full legacy framework) while the code targets the .NET Core framework or other. This is done by 
        /// transforming the <paramref name="message"/> by <see cref="TransformErrorMessage(string)"/>.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not 
        /// a null reference, the current exception is raised in a catch block that handles the inner exception.
        /// </param>
        /// <seealso cref="TransformErrorMessage(string)"/>
        public FrameworkDependencyException(string message, Exception innerException):
            base(TransformErrorMessage(message), innerException)
        {
        }

        /// <summary>Transforms the specified error message - adds information about the roor cause of the error, which is 
        /// dependency on the full .NET Framework while the target framework is something else (usually .NET Core or .NET 5 or higher).</summary>
        /// <param name="message">Message that is transformed.</param>
        /// <returns>Error message (<paramref name="message"/>) supplemented by additional explanation regarding dependency on the full .NET Framework.</returns>
        public static string TransformErrorMessage(string message)
        {

            return "Code depends on .NET Framework, which is not available. Details: " + Environment.NewLine
                + "  " + message != null ? message : "";
        }


    }
}
