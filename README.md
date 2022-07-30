# Nanopass#

A C# reimagining/adaptation of the [Nanopass Framework](http://nanopass.org/). 

Works by providing a path to a C# project or directory containing a project, and a path to a YAML file containing definitions for "passes". The tool will generate source code for new types. Currently record classes are the only supported types for modification, and a language version >= C# 10 is required.

> **Warning**
> This project is still **buggy and sloppy as hell**, currently **does not work as intended**, and **is missing extensive documentation**. The source code is public for the sake of people who have showed interest in using it and if anyone wants to contribute to it.

## Things that currently do not work
[ ] Generated source code formatting

[ ] Writing generated source code to source files in project directory

[ ] Extensive information and error logging (currently done through yanky `Result<T>` type)

[ ] Adding nested types in modification passes (only *removing* types is currently supported)

[ ] Properly marking types as `abstract`/`sealed`

[ ] Bugs, tons of bugs
