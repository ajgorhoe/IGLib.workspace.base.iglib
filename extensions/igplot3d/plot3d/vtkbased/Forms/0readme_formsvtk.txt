
This module contains various Windows Forms that encapsulate VTK rendering window (provided via ActiViz wrappers).

One of the implementation goals was to overcome problems posed by the fact that Windows Forms Designer
can not deal with the VTK control IVtkFormContainer<Kitware.VTK.RenderWindowControl because its library is
provided in pure 64 bit form while the designer requires that control libraries must (also) target the  
32 bit platform.

VTKFORMSDESIGN:
Solution is provided in term of conditional compiling controlled by the compiler variable VTKFORMSDESIGN.
If VTKFORMSDESIGN is set then controls are such that they do not use the VTK control. This is mainly solved
in the VtkControlBase class, then in definition of the IVtkFormContainer interface where type of the eventual
VTK control is controled by the compiler variable, and further in implementation of the IVtkFormContainer
interface in more complex classes (i.e. controls and forms that contain a control of the VtkControlBase type).

FOR RELEASE VERSIONS:  Undefine VTKFORMSDESIGN (e.g. in project properties, add some numbers to this compiler
variable, so that the variable itself gets actually undefined).

FOR DESIGNING CONTROLS AND FORMS: Define VTKFORMSDESIGN
  - in Project/Properties, under Build / General, add variable under "Conditional compilation symbols".


