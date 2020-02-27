# Consul Extensions Configuration for .NET Core by Weknow

## Goals

The Goals of this implementation are:  
* Provide easy registration of Consul as .NET Configuration.
* Seamless registration strong configuration type by convention.
* Multi layer (hierarchical) configuration editing  
  Different aspect of same configuration unit can be edit on different layers.  
  The final configuration will be a merge of all the layers.
  The layers hierarchy is (example: logging activation may be set on different layers from general to specific layer):
  * Configuration Type [Required]: derived from the strong configuration class.  
  * Tenant [Optional]: organization level (support for multi tenant).  
    Enable to mutate configuration at the Tenant level.
  * Application / Micro-Service [Optional]: application or micro-service level.
    Enable to mutate configuration at the Application / Micro-Service level. 
  * Namespace [Optional]: enable to mutate the configuration on each level of a namespace.
  * Component [Optional]: enable to mutate the configuration for specific consumer class.
* Auto reload configuration (the reload cycle will be configurable).
