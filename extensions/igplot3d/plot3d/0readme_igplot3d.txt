

Conditional compilation symbols used: 
  VTKFORMSDESIGN999   // not used any more!


VTK linkage: must perform conditional linkage! Edit .csproj manually!
See also: http://stackoverflow.com/questions/1997268/how-to-reference-different-version-of-dll-with-msbuild 
Within   <ItemGroup>, add the following:
  <ItemGroup>

    <Reference Include="Kitware.mummy.Runtime, Version=1.0.2.599, Culture=neutral, PublicKeyToken=995c7fb9db2c1b44, processorArchitecture=x86" Condition="'$(Platform)'!='x86'" >
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\..\externalextended\ActiViz\bin\Kitware.mummy.Runtime.dll</HintPath>
    </Reference>
    <Reference Include="Kitware.VTK, Version=5.6.1.599, Culture=neutral, PublicKeyToken=995c7fb9db2c1b44, processorArchitecture=x86" Condition="'$(Platform)'!='x86'" >
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\..\externalextended\ActiViz\bin\Kitware.VTK.dll</HintPath>
    </Reference>

    <Reference Include="Kitware.mummy.Runtime, Version=1.0.1.0, Culture=neutral, PublicKeyToken=995c7fb9db2c1b44, processorArchitecture=AMD64" Condition="'$(Platform)'=='x86'" >
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\..\externalextended\ActiViz_5.6.1_x86\bin\Kitware.mummy.Runtime.dll</HintPath>
    </Reference>
    <Reference Include="Kitware.VTK, Version=5.6.1.599, Culture=neutral, PublicKeyToken=995c7fb9db2c1b44, processorArchitecture=AMD64" Condition="'$(Platform)'=='x86'" >
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\..\externalextended\ActiViz_5.6.1_x86\bin\Kitware.VTK.dll</HintPath>
    </Reference>

	...
	...
  </ItemGroup>









