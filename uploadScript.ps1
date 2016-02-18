cd C:\Users\RogerB\Desktop\UploadFiles\SharePointRestLibrary\bin\debug
ipmo .\SharePointRestLibrary.dll

$SPUploadSession = New-SPUploadSession `
    -LocalFolder "d:\" `
    -DomainUserName "baaqmd\roger.boone" `
    -DomainPassword "1.Greatb155" `
    -BaseSharePointUrl "http://baaqmd-records.westus.cloudapp.azure.com/permitting/applications/" `
    -LibraryTitle "Permit Applications" `
    -ContentType "Permit Application" `
    -DBConnectionString "Server = rogerb-pc\sqlexpress; Database = baaqmd_files; User Id = sa; Password = 1.password;" `
    -SelectStatement "select * from vw_pas" `
    -FileNameField "file_name"
    
$SPUploadSession = Add-SPMapping `
    -Session $SPUploadSession `
    -DBSourceField "Application Title" `
    -SPDestinationField "Title" `
    -SPDataType "Text"

$SPUploadSession = Add-SPMapping `
    -Session $SPUploadSession `
    -DBSourceField "Application Number" `
    -SPDestinationField "Application Number" `
    -SPDataType "Text"

$SPUploadSession = Add-SPMapping `
    -Session $SPUploadSession `
    -DBSourceField "Application Title" `
    -SPDestinationField "Application Title" `
    -SPDataType "Text"


$SPUploadSession = Add-SPMapping `
    -Session $SPUploadSession `
    -DBSourceField "Site Number" `
    -SPDestinationField "Site Number" `
    -SPDataType "Text"

$SPUploadSession = Add-SPMapping `
    -Session $SPUploadSession `
    -DBSourceField "Plant Number" `
    -SPDestinationField "Plant Number" `
    -SPDataType "Numeric"

$SPUploadSession = Add-SPMapping `
    -Session $SPUploadSession `
    -DBSourceField "Facility Name" `
    -SPDestinationField "Facility Name" `
    -SPDataType "Text"


$SPUploadSession = Add-SPMapping `
    -Session $SPUploadSession `
    -DBSourceField "Engineer" `
    -SPDestinationField "Engineer" `
    -SPDataType "Text"

$SPUploadSession = Add-SPMapping `
    -Session $SPUploadSession `
    -DBSourceField "PA Status" `
    -SPDestinationField "PA Status" `
    -SPDataType "Taxonomy"

$SPUploadSession = Add-SPMapping `
    -Session $SPUploadSession `
    -DBSourceField "Status Date" `
    -SPDestinationField "Status Date" `
    -SPDataType "Date"

$SPUploadSession = Invoke-SPUpload -Session $SPUploadSession
