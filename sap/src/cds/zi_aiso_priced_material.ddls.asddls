@AccessControl.authorizationCheck: #NOT_REQUIRED
@Metadata.ignorePropagatedAnnotations: true
@EndUserText.label: 'Material có giá PR00 hợp lệ'
define view entity ZI_AISO_PRICED_MATERIAL
  as select distinct from a305
  inner join konp on konp.knumh = a305.knumh
{
  a305.vkorg as SalesOrg,
  a305.vtweg as DistChannel,
  a305.matnr as Material,
  cast( a305.kunnr as kunnr_v ) as Customer,
  cast( '' as waerk )           as Currency
}
where a305.kappl = 'V'
  and a305.kschl = 'PR00'
  and a305.kfrst = ''
  and a305.datab <= $session.system_date
  and a305.datbi >= $session.system_date
  and konp.loevm_ko = ''
  and konp.kbetr > 0

union

select distinct from a304
  inner join konp on konp.knumh = a304.knumh
{
  a304.vkorg as SalesOrg,
  a304.vtweg as DistChannel,
  a304.matnr as Material,
  cast( '' as kunnr_v ) as Customer,
  cast( '' as waerk )   as Currency
}
where a304.kappl = 'V'
  and a304.kschl = 'PR00'
  and a304.kfrst = ''
  and a304.datab <= $session.system_date
  and a304.datbi >= $session.system_date
  and konp.loevm_ko = ''
  and konp.kbetr > 0

union

select distinct from a306
  inner join konp on konp.knumh = a306.knumh
  inner join knvv on knvv.vkorg = a306.vkorg
                 and knvv.vtweg = a306.vtweg
                 and knvv.pltyp = a306.pltyp
{
  a306.vkorg as SalesOrg,
  a306.vtweg as DistChannel,
  a306.matnr as Material,
  cast( knvv.kunnr as kunnr_v ) as Customer,
  cast( a306.waerk as waerk )   as Currency
}
where a306.kappl = 'V'
  and a306.kschl = 'PR00'
  and a306.kfrst = ''
  and a306.pltyp <> ''
  and a306.datab <= $session.system_date
  and a306.datbi >= $session.system_date
  and konp.loevm_ko = ''
  and konp.kbetr > 0 
