########################################################################################
#    _    _                           _                                                #
#   | |  | |                         | |                                               # 
#   | |__| | __ _ _ __  _ __  _   _  | |     __ _ _ __   __ _ _   _  __ _  __ _  ___   #
#   |  __  |/ _` | '_ \| '_ \| | | | | |    / _` | '_ \ / _` | | | |/ _` |/ _` |/ _ \  #
#   | |  | | (_| | |_) | |_) | |_| | | |___| (_| | | | | (_| | |_| | (_| | (_| |  __/  #
#   |_|  |_|\__,_| .__/| .__/ \__, | |______\__,_|_| |_|\__, |\__,_|\__,_|\__, |\___|  #
#                | |   | |     __/ |                     __/ |             __/ |       #
#                |_|   |_|    |___/                     |___/             |___/        #
#                                                                                      #
########################################################################################

This is a Happy Language compiler. This program was created as part of the semestral work for subject KIV/FJP at University of the West Bohemia.

******** Folder structure ********
------ bin - Binaries
  |      |
  |      \--- Happy_language.exe - executable file of this program
  |
  |--- doc - Documentation
  |
  |--- Happy_language - C# project of the compiler
  |      |
  |      \--- Grammar.g4 - Grammar of the Happy Language
  |
  |--- Happy_language.Tests - C# project of the Tests
  |
  |--- packages - Dependencies of given C# projects
  |
  |--- TestFiles - Test files of Happy Language used in Test project. Feel free to use them as reference of
  |     what Happy Language can do. Instructions generated from them are also present
  |
  |--- build.bat - Script to build this program. See comipiling instructions for more info
  |
  |--- Happy_language.sln - Visual studio 15 solution for the whole program
  |
  \--- refint_pl0_ext.exe - Modified interpreter of the Extended PL/0 that executes instructions
		generated by the Happy_language compiler.
		
		
***** Compiling instruction ******
This program works on Windows. No testing with MONO on Linux was made.

1) Be sure to have installed .NET Framework 4.5 (it is better to have installed Visual Studio 2015)
2) Be sure that you have path to the bin folder containing MSBuild.exe in your %PATH% environment variable
		(see https://social.msdn.microsoft.com/Forums/windowsapps/en-US/23a7dc5d-c337-4eed-8af4-c016def5516e/location-of-msbuildexe?forum=msbuild if any problems with locating the folder)
3) Run build.bat file
4) Executable with its DLL dependency is in the bin folder


****** Running instruction ******
HappyLanguage.exe <input file> [output file]
        input file: File from which the code will be parsed.
        output file: File to which the result will be written. If no output file is specified, 'out.pl0' is used.

or

HappyLanguage.exe -h
         to print help