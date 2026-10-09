@AbapCatalog.viewEnhancementCategory: [#NONE]
@AccessControl.authorizationCheck: #NOT_REQUIRED
@EndUserText.label: 'Material có giá (consumption)'
@Metadata.ignorePropagatedAnnotations: true
define view entity ZC_AISO_PRICED_MATERIAL
  as select from ZI_AISO_PRICED_MATERIAL
{
  key SalesOrg,
  key DistChannel,
  key Material,
  key Customer,
  key Currency
}
