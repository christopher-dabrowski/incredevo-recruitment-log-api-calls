@description('Base name for all resources')
param baseName string = 'fnapp${uniqueString(resourceGroup().id)}'

@description('Storage Account type')
@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_RAGRS'
])
param storageAccountType string = 'Standard_LRS'

@description('Location for all resources.')
param location string = resourceGroup().location

@description('Id of the AD Tenant whre infrastructure is deployed')
param infrasturcutreTenantId string = tenant().tenantId

param applicationInsightsName string = '${baseName}-appi'
param storageAccountName string = '${take(toLower(replace(replace(baseName, '-', ''), '_', '')), 24)}sa'
param hostingPlanName string = '${baseName}-asp'
param functionAppName string = '${baseName}-func'
param keyVaultName string = take('${baseName}-kv', 24)

var functionWorkerRuntime = 'dotnet-isolated'

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: storageAccountType
  }
  kind: 'Storage'
  properties: {
    supportsHttpsTrafficOnly: true
    defaultToOAuthAuthentication: true
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: hostingPlanName
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  properties: {}
}

module functionApp './modules/azureFuncion.bicep' = {
  name: 'funcionApp'
  params: {
    appServciePlanName: hostingPlan.name
    location: location
    name: functionAppName
  }
}

module funcionAppSettings './modules/azureFuncionSettings.bicep' = {
  name: 'funcionAppSettings'
  params: {
    keyVaultName: keyVault.outputs.keyVaultName
    parentName: functionApp.outputs.azureFuncionName
    runtime: functionWorkerRuntime
  }
}

module keyVault './modules/keyVault.bicep' = {
  name: 'keyVault'
  params: {
    name: keyVaultName
    location: location
    infrasturcutreTenantId: infrasturcutreTenantId
    storageAccountName: storageAccount.name
    applicationInsightsName: applicationInsights.name
    readers: [ functionApp.outputs.objectId ]
  }
}
