
Project igforms.

BASIC ON FORMS AND CONTROLS:
  Only the following members are thread safe: BeginInvoke, EndInvoke, Invoke, InvokeRequired.
  Application.Run(Form mainForm); begins a message pump (event loop) and shows the form.
throws exception if message pump is already running.
On main GUI thread, you can create and show forms by running:
  Form f = new Form(); f.ShowDialog();  - opens a modal form. If the form open in this 
      way is closed by the user via the window's close button then the form is not 
	  destroyed but only Visible is set to false, and DialogResult.Cancel is returned.
or 
  Form f = new Form(); f.Show(); - opens a form in non-modal way. 
    This is equivalent to f.Visible = true;

SystemInformation.UserInteractive property gets a value indicating whether the current 
   process is running in user-interactive mode. If false then modal dialogs may not be shown!

FORMS IN MULTITHREADED ENVIRONMENT

In .NET, all GUI components and their methods must be accessed from a single thread
that contains the event loop. The main method of the project using forms should 
have the attribute [STAThread].

GUI components whose services can be used from different threads must be adapted in the 
following ways

CREATING FORMS FROM PARALLEL THREADS:
private static Form MainForm; // set this to your main form
private void SomethingOnBackgroundThread() {
    string someData = "some data";
    MainForm.BeginInvoke((Action)delegate {
        var form = new MyForm();
        form.Text = someData;
        form.Show();
    });
}
There exists one other possibility - to call Applocation.Run() on parallel threads,
in this case each thread will have its own message pump and can open its own forms
that will run event handler on these different parallel threads, but this is not
recommendable. This is done iin the following way:
Form f = new Form(); Application.Run(f)
- the above code can be run in multiple parallel threads, causing that each thread will
have its own message pump and its own set of forms whose event handlers will execute
on that thread (meaning also that several modal dialogs can be open in parallel by
the f.ShowDialoG() method!)

ACCESSING GUI COMPONENTS FROM PARALLEL THREADS
  See this article (problems may be shared resources!):
http://stackoverflow.com/questions/1909839/invoke-and-begininvoke
  One possibility is to introduce a lock on GUI controls accessed from multiple threads,
but CONTROLS MAY NOT BE LOCKED BY EVENT HANDLERS and code called with them, so you must
strictly separate between the code that si called from elsewhere and code called in 
the message loop (event loop)!

  All control's methods that are not called from control's event handlers but access the 
form's components and properties, must test by InvokeRequired, and use the Invoke() or
BeginInvoke() to execute the code body!! This transfers execution to the thread that does
the message pump (equivalent to event loop) for the control in question! Consider vert





