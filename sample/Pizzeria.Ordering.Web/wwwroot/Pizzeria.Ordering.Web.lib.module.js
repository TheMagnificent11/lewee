export async function onRuntimeConfigLoaded(config) {
    const configUrl = new URL("_blazor/_configuration", document.baseURI);
    const response = await fetch(configUrl);
    if (!response.ok) {
        throw new Error(`Failed to load Aspire client configuration: ${response.status}`);
    }

    const serverConfig = await response.json();
    const environment = serverConfig?.webAssembly?.environment;
    if (environment) {
        config.environmentVariables = {
            ...config.environmentVariables,
            ...environment,
        };
    }
}
