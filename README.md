# Json to CSharp Class Generator

## Description

A simple .NET Core console application to

## Getting Started

### Dependencies

- Windows OS

### Installing

- No installation is required

## Flags

```
`-p|--path` (string) the path to scan from
`-s|-sizeout` (number) the number of directories to show in output
`-v|--verbose` (no value) include verbose output
```

## Usage

#### Input .json file

`.\JsonToClassConverter.exe -j $escapedJson -o C:\fv\csharpout.cs`

#### Input json string

`.\JsonToClassConverter.exe -i C:\fv\input_file.json -o C:\fv\csharpout.cs`

#### Input url with json response

`.\JsonToClassConverter.exe -u "https://jsonplaceholder.typicode.com/todos/1" -o C:\fv\csharpout.cs`

#### Use as part of a script

```
$apiUrl = "https://jsonplaceholder.typicode.com/todos/1"
$jsonResponse = Invoke-RestMethod -Uri $apiUrl -Method Get

$jsonString = $jsonResponse | ConvertTo-Json -Depth 10
$escapedJson = $jsonString -replace '"', '\"'

.\JsonToClassConverter.exe -j $escapedJson -o C:\fv\csharpout.cs
```
