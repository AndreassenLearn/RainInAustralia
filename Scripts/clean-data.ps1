$pathOriginal = "C:\vs\RainInAustralia\Data\weather-original.csv"
$pathNoNa = "C:\vs\RainInAustralia\Data\weather-no-na.csv"
$pathFinal = "C:\vs\RainInAustralia\Data\weather-final.csv"
$pathValidation = "C:\vs\RainInAustralia\Data\weather-validation.csv"
$pathTrain = "C:\vs\RainInAustralia\Data\weather-train.csv"

# Import and remove all rows that contains the "NA" value in the specified columns.
echo "Removing 'NA' values..."
$noNaCSV = Import-CSV $pathOriginal | Where-Object {
	($_.MinTemp -notcontains "NA") -and
	($_.MaxTemp -notcontains "NA") -and
	($_.Rainfall -notcontains "NA") -and
	($_.WindGustDir -notcontains "NA") -and
	($_.WindGustSpeed -notcontains "NA") -and
	($_.WindDir9am -notcontains "NA") -and
	($_.WindDir3pm -notcontains "NA") -and
	($_.WindSpeed9am -notcontains "NA") -and
	($_.WindSpeed3pm -notcontains "NA") -and
	($_.Humidity9am -notcontains "NA") -and
	($_.Humidity3pm -notcontains "NA") -and
	($_.Pressure9am -notcontains "NA") -and
	($_.Pressure3pm -notcontains "NA") -and
	($_.Temp9am -notcontains "NA") -and
	($_.Temp3pm -notcontains "NA") -and
	($_.RainToday -notcontains "NA") -and
	($_.RainTomorrow -notcontains "NA")
}

# Export no NA.
echo "Done. Exporting to: $pathNoNa"
$noNaCSV | Export-Csv -NoTypeInformation $pathNoNa

# Format columns.
echo "Formatting columns..."
$finalCSV = Import-CSV $pathNoNa | ForEach-Object {
	#$_.Date = $_.Date.Substring(5,2) # Only month number.
	#$_.Date = $_.Date -replace '01','1'
	#$_.Date = $_.Date -replace '02','2'
	#$_.Date = $_.Date -replace '03','3'
	#$_.Date = $_.Date -replace '04','4'
	#$_.Date = $_.Date -replace '05','5'
	#$_.Date = $_.Date -replace '06','6'
	#$_.Date = $_.Date -replace '07','7'
	#$_.Date = $_.Date -replace '08','8'
	#$_.Date = $_.Date -replace '09','9'
	$_.WindGustDir = $_.WindGustDir -replace 'NNE','22.5'
	$_.WindGustDir = $_.WindGustDir -replace 'ENE','67.5'
	$_.WindGustDir = $_.WindGustDir -replace 'ESE','112.5'
	$_.WindGustDir = $_.WindGustDir -replace 'SSE','157.5'
	$_.WindGustDir = $_.WindGustDir -replace 'SSW','202.5'
	$_.WindGustDir = $_.WindGustDir -replace 'WSW','247.5'
	$_.WindGustDir = $_.WindGustDir -replace 'WNW','292.5'
	$_.WindGustDir = $_.WindGustDir -replace 'NNW','337.5'
	$_.WindGustDir = $_.WindGustDir -replace 'NE','45'
	$_.WindGustDir = $_.WindGustDir -replace 'SE','135'
	$_.WindGustDir = $_.WindGustDir -replace 'SW','225'
	$_.WindGustDir = $_.WindGustDir -replace 'NW','315'
	$_.WindGustDir = $_.WindGustDir -replace 'N','0'
	$_.WindGustDir = $_.WindGustDir -replace 'E','90'
	$_.WindGustDir = $_.WindGustDir -replace 'S','180'
	$_.WindGustDir = $_.WindGustDir -replace 'W','270'
	$_.WindDir9am = $_.WindDir9am -replace 'NNE','22.5'
	$_.WindDir9am = $_.WindDir9am -replace 'ENE','67.5'
	$_.WindDir9am = $_.WindDir9am -replace 'ESE','112.5'
	$_.WindDir9am = $_.WindDir9am -replace 'SSE','157.5'
	$_.WindDir9am = $_.WindDir9am -replace 'SSW','202.5'
	$_.WindDir9am = $_.WindDir9am -replace 'WSW','247.5'
	$_.WindDir9am = $_.WindDir9am -replace 'WNW','292.5'
	$_.WindDir9am = $_.WindDir9am -replace 'NNW','337.5'
	$_.WindDir9am = $_.WindDir9am -replace 'NE','45'
	$_.WindDir9am = $_.WindDir9am -replace 'SE','135'
	$_.WindDir9am = $_.WindDir9am -replace 'SW','225'
	$_.WindDir9am = $_.WindDir9am -replace 'NW','315'
	$_.WindDir9am = $_.WindDir9am -replace 'N','0'
	$_.WindDir9am = $_.WindDir9am -replace 'E','90'
	$_.WindDir9am = $_.WindDir9am -replace 'S','180'
	$_.WindDir9am = $_.WindDir9am -replace 'W','270'
	$_.WindDir3pm = $_.WindDir3pm -replace 'NNE','22.5'
	$_.WindDir3pm = $_.WindDir3pm -replace 'ENE','67.5'
	$_.WindDir3pm = $_.WindDir3pm -replace 'ESE','112.5'
	$_.WindDir3pm = $_.WindDir3pm -replace 'SSE','157.5'
	$_.WindDir3pm = $_.WindDir3pm -replace 'SSW','202.5'
	$_.WindDir3pm = $_.WindDir3pm -replace 'WSW','247.5'
	$_.WindDir3pm = $_.WindDir3pm -replace 'WNW','292.5'
	$_.WindDir3pm = $_.WindDir3pm -replace 'NNW','337.5'
	$_.WindDir3pm = $_.WindDir3pm -replace 'NE','45'
	$_.WindDir3pm = $_.WindDir3pm -replace 'SE','135'
	$_.WindDir3pm = $_.WindDir3pm -replace 'SW','225'
	$_.WindDir3pm = $_.WindDir3pm -replace 'NW','315'
	$_.WindDir3pm = $_.WindDir3pm -replace 'N','0'
	$_.WindDir3pm = $_.WindDir3pm -replace 'E','90'
	$_.WindDir3pm = $_.WindDir3pm -replace 'S','180'
	$_.WindDir3pm = $_.WindDir3pm -replace 'W','270'
	$_.RainToday = $_.RainToday -replace 'Yes','true'
	$_.RainToday = $_.RainToday -replace 'No','false'
	$_.RainTomorrow = $_.RainTomorrow -replace 'Yes','true'
	$_.RainTomorrow = $_.RainTomorrow -replace 'No','false'
	$_ # Feed back.
}

# Export final.
echo "Done. Exporting to: $pathFinal"
$finalCSV | Export-Csv -NoTypeInformation $pathFinal

# Get samples for validation.
echo "Selecting validation samples..."
$validationCSV = Import-CSV $pathFinal | Get-Random -Count 1000 | Sort Location,Date

# Export validation data.
echo "Done. Exporting to: $pathValidation"
$validationCSV | Export-Csv -NoTypeInformation $pathValidation

# Get train data.
echo "Selecting training samples..."
$trainCSV = Import-CSV $pathFinal | Where-Object {
	#@(Compare-Object $_ $validationCSV -Property Date,Location,MinTemp,MaxTemp,Rainfall,WindGustDir,WindGustSpeed,WindDir9am,WindDir3pm,WindSpeed9am,WindSpeed3pm,Humidity9am,Humidity3pm,Pressure9am,Pressure3pm,Temp9am,Temp3pm -IncludeEqual -ExcludeDifferent).count -eq 0
	@(Compare-Object $_ $validationCSV -Property Date,Location -IncludeEqual -ExcludeDifferent).count -eq 0
} | Sort Location,Date

# Export train data.
echo "Done. Exporting to: $pathTrain"
$trainCSV | Export-Csv -NoTypeInformation $pathTrain
