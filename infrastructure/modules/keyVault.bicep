param name string
param location string
param infrasturcutreTenantId string

param storageAccountName string
param applicationInsightsName string

param readers array

var accessPolicies = map(readers, readerId => {
    tenantId: infrasturcutreTenantId
    objectId: readerId
    permissions: {
      secrets: [
        'get'
        'list'
      ]
    }
  })

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: name
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: infrasturcutreTenantId
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    enableRbacAuthorization: false
    provisioningState: 'Succeeded'
    publicNetworkAccess: 'Enabled'

    accessPolicies: accessPolicies
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' existing = {
  name: storageAccountName
}

var storageAccountKey = storageAccount.listKeys().keys[0].value
var storageAccountConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${storageAccountKey};EndpointSuffix=${environment().suffixes.storage}'

resource storageAccountConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: keyVault
  name: 'StorageAccount--ConnectionString'
  properties: {
    value: storageAccountConnectionString
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: applicationInsightsName
}

resource applicationInsightsIntrumentationKeySecret 'Microsoft.KeyVault/vaults/secrets@2022-07-01' = {
  parent: keyVault
  name: 'ApplicationInsights--InstrumentationKey'
  properties: {
    value: applicationInsights.properties.InstrumentationKey
  }
}

output keyVaultName string = keyVault.name
