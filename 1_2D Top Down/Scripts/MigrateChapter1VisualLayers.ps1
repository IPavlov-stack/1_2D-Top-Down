param(
    [string[]]$MapPaths = @(
        "Content/Maps/Chapter1/Mission01.tmx",
        "Content/Maps/Chapter1/Mission02.tmx"
    )
)

$renameMap = @{
    GroundTransitions = "MainSpace"
    GroundSpots       = "Spots1"
    RockSpots         = "Spots3"
    GrassStairs       = "Stairs"
}

$visualOrder = @(
    "Water", "WaterDetails", "WaterDetails2", "Ground",
    "Spots1", "Spots2", "MainSpace", "MainSpace2", "WaterLilies",
    "Shadow", "ObjectsUnderElevatedSpace", "ElevatedSpace", "Spots3",
    "Stairs", "GrassElements", "GrassElements2", "GrassElements3",
    "Lianas5", "Lianas", "Lianas2", "Lianas3", "Lianas4",
    "Objects2", "Objects1", "Objects3", "Objects4", "Objects5", "Reeds"
)

foreach ($mapPath in $MapPaths) {
    $fullPath = [System.IO.Path]::GetFullPath($mapPath)
    $document = [System.Xml.Linq.XDocument]::Load(
        $fullPath,
        [System.Xml.Linq.LoadOptions]::PreserveWhitespace)
    $map = $document.Root
    $width = [int]$map.Attribute("width").Value
    $height = [int]$map.Attribute("height").Value

    foreach ($entry in $renameMap.GetEnumerator()) {
        $layer = $map.Elements("layer") |
            Where-Object { $_.Attribute("name").Value -eq $entry.Key } |
            Select-Object -First 1
        if ($null -ne $layer) {
            $layer.SetAttributeValue("name", $entry.Value)
        }
    }

    $nextLayerId = [int]$map.Attribute("nextlayerid").Value
    $row = (0..($width - 1) | ForEach-Object { "0" }) -join ","
    $csv = (0..($height - 1) | ForEach-Object { $row }) -join ",`n"

    foreach ($layerName in $visualOrder) {
        $existing = $map.Elements("layer") |
            Where-Object { $_.Attribute("name").Value -eq $layerName } |
            Select-Object -First 1
        if ($null -ne $existing) {
            continue
        }

        $layer = [System.Xml.Linq.XElement]::new(
            "layer",
            [System.Xml.Linq.XAttribute]::new("id", $nextLayerId),
            [System.Xml.Linq.XAttribute]::new("name", $layerName),
            [System.Xml.Linq.XAttribute]::new("width", $width),
            [System.Xml.Linq.XAttribute]::new("height", $height),
            [System.Xml.Linq.XText]::new("`n  "),
            [System.Xml.Linq.XElement]::new(
                "data",
                [System.Xml.Linq.XAttribute]::new("encoding", "csv"),
                [System.Xml.Linq.XText]::new("`n$csv`n")),
            [System.Xml.Linq.XText]::new("`n "))
        $map.Add($layer)
        $nextLayerId++
    }

    $orderedLayers = foreach ($layerName in $visualOrder) {
        $map.Elements("layer") |
            Where-Object { $_.Attribute("name").Value -eq $layerName } |
            Select-Object -First 1
    }

    foreach ($layer in $orderedLayers) {
        $layer.Remove()
    }

    $firstObjectGroup = $map.Elements("objectgroup") | Select-Object -First 1
    foreach ($layer in $orderedLayers) {
        if ($null -ne $firstObjectGroup) {
            $firstObjectGroup.AddBeforeSelf($layer)
            $firstObjectGroup.AddBeforeSelf([System.Xml.Linq.XText]::new("`n "))
        }
        else {
            $map.Add($layer)
        }
    }

    $map.SetAttributeValue("nextlayerid", $nextLayerId)
    $document.Save($fullPath, [System.Xml.Linq.SaveOptions]::DisableFormatting)
}
