
$json = @();

# get all files in the srcfiles folder
$files = Get-ChildItem -Path ".\pnppsdocs" -Filter "*.md" -Recurse;

# set id to 1
$id = 1;

# loop through each file
$files | ForEach-Object {
    
    # get file name without extension
    $baseName = $_.BaseName;

    # get the file data
    $fileData = Get-Content $_.FullName -Raw;
    # create a regex pattern to match the example code
    $pattern = "(?s)(?<=### EXAMPLE .*``````powershell)(.*?)(?=``````)"

    if($baseName.ToLower() -eq "connect-pnponline") {
        $pattern = "(?s)(?<=### EXAMPLE .*``````)(.*?)(?=``````)";
    }

    $options = [Text.RegularExpressions.RegexOptions]'IgnoreCase, CultureInvariant'


    $result = [regex]::Matches($fileData, $pattern, $options);

    $i = 1;
    foreach ($item in $result) {

        $value = $item.Value.Trim();

        # replace \n with a semicolon
        $value = $value.Replace("`n", " ; ");


        # if the item value begins with the name of the file then add it to the json
        if ($value.ToLower() -match "^$($baseName.ToLower()).*") {
            $json += @{
                "CommandName" = $baseName
                "Command" = $value
                "Rank" = $i
                "Id" = $id
            }
            $i++;
            $id++;
        }
    }

}

# write the json to a file
$json | ConvertTo-Json -Depth 10 | Out-File -FilePath ".\PnP.PowerShell.Suggestions.json" -Encoding UTF8 -Force;