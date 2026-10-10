variable "prefix" {
  description = "Prefix used for every resource, student code"
  type        = string
}

variable "location" {
  type    = string
  default = "austriaeast"
}

variable "subscription_id" {
  type = string
}

variable "app_image" {
  type    = string
  default = "ghcr.io/erpusk/boardgames-api:latest"
}

variable "vm_size" {
  type    = string
  default = "Standard_B2ats_v2"
}

variable "locked" {
  type    = bool
  default = false
}

variable "resource_group_name" {
  description = "Azure resource group name"
  type        = string
  default     = "262957iaim-rg"
}

variable "admin_username" {
  description = "Linux VM administrator username"
  type        = string
  default     = "azureuser"
}

variable "ssh_public_key" {
  description = "SSH public key used to access the VM"
  type        = string
}

variable "ssh_source_ip" {
  description = "Public IP address allowed to SSH to the VM, in CIDR notation"
  type        = string
}