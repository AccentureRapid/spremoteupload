$SPUploadSession = New-SPUploadSession `
    -LocalFolder "C:\testDocuments\" `
    -DomainUserName baaqmd-dev\sp_farm `
    -DomainPassword "P@$$word1!" `
    -BaseSharePointUrl "http://baaqmd-dev.cloudapp.net/sites/RecordCenter/" `
    -LibraryTitle "Multi Test Library" `
    -DBConnectionString "Server = rogerb-pc\sqlexpress; Database = baaqmd_files; User Id = sa; Password = 1.password;" `
    -SelectStatement "Select * from GeneralLedger where somefield='cancelled'" `
    -FileNameField file_name `
    -ContentType = "PA - Cancelled"
    
$SPUploadSession = Add-SPMapping `
    -Session $SPUploadSession `
    -DBSourceField "Title" `
    -SPDestinationField "Title" `
    -SPDataType "Text"

$SPUploadSession = Add-SPMapping `
    -Session $SPUploadSession `
    -DBSourceField "Book Number" `
    -SPDestinationField "Book_x0020_Number" `
    -SPDataType "Numeric"

$SPUploadSession = Add-SPMapping `
    -Session $SPUploadSession `
    -DBSourceField "Retention Date" `
    -SPDestinationField "Retention_x0020_Date" `
    -SPDataType "Date"

$SPUploadSession = Invoke-SPUpload -Session $SPUploadSession

