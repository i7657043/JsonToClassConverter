# Json to CSharp Class Generator (json2csharp.exe)

### Description

A simple .NET Core console application to generate CSharp classes from JSON

### Dependencies

- Windows OS

### Installing

- No installation is required

## Flags

```
Select 1 of these options
`-u|--url` (string) url with JSON response to use as input
`-j|--json` (string) raw JSON string to use as input
`-f|-file` (string) path to JSON file to use as input

Required
`-o|--out` (no value) path to write CSharp class files to
```

## Usage

#### Input url with json response

`.\json2csharp.exe -u "https://petition.parliament.uk/petitions/700143.json" -o C:\fv\csharpout.cs`

#### Input json string

```
//Use as part of a script
$apiUrl = "https://petition.parliament.uk/petitions/700143.json"
$jsonResponse = Invoke-RestMethod -Uri $apiUrl -Method Get

$jsonString = $jsonResponse | ConvertTo-Json -Depth 10
$escapedJson = $jsonString -replace '"', '\"'

.\json2csharp.exe -j $escapedJson -o C:\fv\csharpout.cs
```

#### Input .json file

`.\json2csharp.exe -i C:\fv\input_file.json -o C:\fv\csharpout.cs`
