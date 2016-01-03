[T4Scaffolding.ControllerScaffolder("Controller with read/write action and views, using repositories", HideInConsole = $true, Description = "Adds an ASP.NET MVC controller with views and data access code", SupportsModelType = $true, SupportsDataContextType = $true, SupportsViewScaffolder = $true)][CmdletBinding()]
param(     
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][string]$ControllerName,   
	[string]$ModelType,
    [string]$Project,
    [string]$CodeLanguage,
	[string]$DbContextType,
	[string]$Area,
	[string]$ViewScaffolder = "View",
	[alias("MasterPage")]$Layout,
 	[alias("ContentPlaceholderIDs")][string[]]$SectionNames,
	[alias("PrimaryContentPlaceholderID")][string]$PrimarySectionName,
	[switch]$ReferenceScriptLibraries = $false,
	[switch]$NoChildItems = $false,
	[string[]]$TemplateFolders,
	[switch]$Force = $false,
	[string]$ForceMode,
	[string]$ViewModel,
	[switch]$Repository = $false
)



Scaffold MvcScaffolding.Controller `
	-ControllerName $ControllerName `
	-ModelType $ModelType `
    -Project $Project `
    -CodeLanguage $CodeLanguage `
	-DbContextType $DbContextType `
	-Area $Area `
	-ViewScaffolder $ViewScaffolder `
	-Layout $Layout `
 	-SectionNames $SectionNames `
	-PrimarySectionName $PrimarySectionName `
	-ReferenceScriptLibraries:$ReferenceScriptLibraries `
	-NoChildItems:$NoChildItems `
	-OverrideTemplateFolders $TemplateFolders `
	-Force:$Force `
	-ForceMode $ForceMode `
	-Repository $Repository

	

# Interpret the "Force" and "ForceMode" options
$overwriteController = $Force -and ((!$ForceMode) -or ($ForceMode -eq "ControllerOnly"))
$overwriteFilesExceptController = $Force -and ((!$ForceMode) -or ($ForceMode -eq "PreserveController"))

# If you haven't specified a model type, we'll guess from the controller name
if (!$ModelType) {
	if ($ControllerName.EndsWith("Controller", [StringComparison]::OrdinalIgnoreCase)) {
		# If you've given "PeopleController" as the full controller name, we're looking for a model called People or Person
		$ModelType = [System.Text.RegularExpressions.Regex]::Replace($ControllerName, "Controller$", "", [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
		$foundModelType = Get-ProjectType $ModelType -Project $Project -ErrorAction SilentlyContinue
		if (!$foundModelType) {
			$ModelType = [string](Get-SingularizedWord $ModelType)
			$foundModelType = Get-ProjectType $ModelType -Project $Project -ErrorAction SilentlyContinue
		}
	} else {
		# If you've given "people" as the controller name, we're looking for a model called People or Person, and the controller will be PeopleController
		$ModelType = $ControllerName
		$foundModelType = Get-ProjectType $ModelType -Project $Project -ErrorAction SilentlyContinue
		if (!$foundModelType) {
			$ModelType = [string](Get-SingularizedWord $ModelType)
			$foundModelType = Get-ProjectType $ModelType -Project $Project -ErrorAction SilentlyContinue
		}
		if ($foundModelType) {
			$ControllerName = [string](Get-PluralizedWord $foundModelType.Name) + "Controller"
		}
	}
	if (!$foundModelType) { throw "Cannot find a model type corresponding to a controller called '$ControllerName'. Try supplying a -ModelType parameter value." }
} else {
	# If you have specified a model type
	$foundModelType = Get-ProjectType $ModelType -Project $Project
	if (!$foundModelType) { return }
	
}



$viewModelClassName=$ModelType+"ViewModel"
$viewModelOutputPath = if ($Area) { "Areas\$Area\Models\$viewModelClassName" } else { "Models\$viewModelClassName" } 



Write-Host "Scaffolding ...$viewModelOutputPath"

$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value

$areaNamespace = if ($Area) { [T4Scaffolding.Namespaces]::Normalize($namespace + ".Areas.$Area") } else { $namespace }

$viewModelNamespace = [T4Scaffolding.Namespaces]::Normalize($areaNamespace + ".Models")
$modelTypeNamespace = [T4Scaffolding.Namespaces]::GetNamespace($foundModelType.FullName)

Add-ProjectItemViaTemplate $viewModelOutputPath -Template Test.ControllerViewModel `
	-Model @{
	ClassName=$ModelType; 
	Namespace = $namespace; 
	ViewModelNamespace = $viewModelNamespace;
	ModelNamespace = $modelTypeNamespace;
	ExampleValue = "Hello, world!";
	ModelType = [MarshalByRefObject]$foundModelType; 
	} `
	-SuccessMessage "Added Test1.Controller output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

$viewName=[string](Get-PluralizedWord $ModelType)
$viewPath = if ($Area) { "Areas\$Area\Views\$viewName\PagedList" } else { "Views\$viewName\PagedList" } 


$foundViewModelType=Get-ProjectType $viewModelClassName -Project $Project -ErrorAction SilentlyContinue
Write-Host "Scaffolding pagedlist view ...$viewPath"

$collectionTypeRelatedEntities = [Array](Get-RelatedEntities $foundModelType.FullName -Project $project)
if (!$collectionTypeRelatedEntities) { $collectionTypeRelatedEntities = @() }
Add-ProjectItemViaTemplate $viewPath -Template Test.PagedList `
	-Model @{
	ViewName=$viewModelClassName; 
	Namespace = $namespace; 
	ViewModelNamespace = $viewModelNamespace;
	ModelNamespace = $modelTypeNamespace;
	ExampleValue = "Hello, world!";
	ModelType = [MarshalByRefObject]$foundViewModelType;
	CollectionType=[MarshalByRefObject]$foundModelType;
	RelatedEntities=$collectionTypeRelatedEntities
	} `
	-SuccessMessage "Added Test.PagedList output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force


