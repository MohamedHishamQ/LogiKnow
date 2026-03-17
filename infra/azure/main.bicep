// Main Bicep template for LogiKnow Platform
targetScope = 'subscription'

param resourceGroupName string = 'rg-logiknow-prod'
param location string = 'eastus'
param sqlServerAdminLogin string = 'logiknowadmin'
@secure()
param sqlServerAdminPassword string

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
}

module sqlServer 'br/public:avm/res/sql/server:0.4.0' = {
  name: 'sqlServerDeployment'
  scope: rg
  params: {
    name: 'sql-logiknow-${uniqueString(rg.id)}'
    administratorLogin: sqlServerAdminLogin
    administratorLoginPassword: sqlServerAdminPassword
    databases: [
      {
        name: 'sqldb-logiknow'
        skuName: 'S0'
        tier: 'Standard'
      }
    ]
  }
}

module appServicePlan 'br/public:avm/res/web/serverfarm:0.2.0' = {
  name: 'appServicePlanDeployment'
  scope: rg
  params: {
    name: 'plan-logiknow-${uniqueString(rg.id)}'
    skuName: 'B1'
    kind: 'linux'
    reserved: true
  }
}

module backendApp 'br/public:avm/res/web/site:0.5.0' = {
  name: 'backendAppDeployment'
  scope: rg
  params: {
    name: 'app-logiknow-api-${uniqueString(rg.id)}'
    kind: 'app,linux'
    serverFarmResourceId: appServicePlan.outputs.resourceId
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|6.0'
      appSettings: [
        {
          name: 'ConnectionStrings__DefaultConnection'
          value: 'Server=tcp:${sqlServer.outputs.name}.database.windows.net,1433;Initial Catalog=sqldb-logiknow;User ID=${sqlServerAdminLogin};Password=${sqlServerAdminPassword};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
        }
      ]
    }
  }
}

module frontendApp 'br/public:avm/res/web/site:0.5.0' = {
  name: 'frontendAppDeployment'
  scope: rg
  params: {
    name: 'app-logiknow-web-${uniqueString(rg.id)}'
    kind: 'app,linux'
    serverFarmResourceId: appServicePlan.outputs.resourceId
    siteConfig: {
      linuxFxVersion: 'NODE|20-lts'
      appSettings: [
        {
          name: 'NEXT_PUBLIC_API_URL'
          value: 'https://${backendApp.outputs.defaultHostname}/api'
        }
      ]
      appCommandLine: 'pm2 start /home/site/wwwroot/server.js --no-daemon'
    }
  }
}

output apiEndpoint string = backendApp.outputs.defaultHostname
output webEndpoint string = frontendApp.outputs.defaultHostname
