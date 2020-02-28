# Consul Extensions Configuration for .NET Core by Weknow
This package provide distributed configuration implementation 
by using HashiCorp’s Consul as the configuration storage, 
via .NET Core configuration system.

## Goals

The Goals are:  
* Provide easy registration of **Consul as .NET Configuration**.
* **Seamless registration** of strong type configuration via naming convention.
* **Multi-layer** (hierarchical) configuration editing  
  Different aspect of same configuration unit can be spread  
  across different layers / hierarchic (like: tenant, service level, environment, namespace).
  The final configuration will be a merge of data across all layers.
  **Layers**:
  * **Configuration Type** \[Required]: represent a **strong configuration class**.  
  * **Tenant** \[Optional]: organization level (support for multi-tenant).  
    Enable to mutate configuration at the Tenant level.
  * **Application or Micro-Service** \[Optional]: application or micro-service level.
    Enable to mutate configuration at this layer. 
  * **Namespace** \[Optional]: enable to mutate the configuration on each level of a namespace.
  * **Component** \[Optional]: enable to mutate the configuration for specific consumer class.
* **Auto reload** configuration when configuration has changed (the reload cycle will be configurable).
