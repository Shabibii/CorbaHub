# CorbaHub
 Final Project (University BEng Computer Science), a supportive tool for the implementation of  the Common Object Request Broker Architecture (CORBA) in Smart Manufacturing

This folder contains the artefact for this project. 
The artefact is accompanied with required CORBA resources for translation of the IDL to CLS (C#), connection over the Internet Inter-ORB Protocol and others.

The program is already compiled and the required resources are generated in the Debug folder (this is done so the program can be run instantly using "CorbaHub.exe"). 

Run the program by initiation (parameters can be added if run through the terminal) in the debug folder through the executable file"CorbaHub.exe". 
The IDL file resides in the same folder, "CorbaHub.idl". 
Provide the IDL file (make sure it is the file in the debug folder, since the generated files are placed here) when the program prompts for it.

The program will run in default mode and generates templates in the created folders "CSharp" and "Java".  
These folders contain the resources for the CORBA protocol, e.g. the stubs, skeletons and object adapters. 

NOTE: CorbaHub requires the "idlj.exe" (available with JDK versions 1-10) file for the generation of CORBA dependencies for Java. 
This file should be available on the user's device. The program searches for the file manually, if not found, it will prompt the user for the JDK path.
