create or replace procedure prc_delete_all_apn_HLR(msisdn in varchar2) is

  Transc  varchar2(27);
  xc      varchar2(20);
  xml     clob;
  
begin
    --DELETING 3G APNs from HLR
    xml:='<ZTEDirectMML><IMSI>410060000000001</IMSI><DirectMML>"Set TPLGPRS: MSISDN=92' || substr(msisdn, -10, 10) || ',GPRSTPL=0;"</DirectMML></ZTEDirectMML>';
   
      
      select TEMP.Nextval into transc from dual;
      oraclesanoc.putrequest(transc,
                            'apnd',
                            '85',
                            '' || msisdn || '',
                            null,
                            null,
                            xml,
                            xc);
                            
    -- DELETING 4G APNs from HLR                        
    xml:='<ZTEDirectMML><IMSI>410060000000001</IMSI><DirectMML>"Set TPLAPNCP:MSISDN=92' || substr(msisdn, -10, 10) || ',EPCAPNCPTPL=0;"</DirectMML></ZTEDirectMML>';
 
      
      select TEMP.Nextval into transc from dual;
      oraclesanoc.putrequest(transc,
                            'apnd',
                            '85',
                            '' || msisdn || '',
                            null,
                            null,
                            xml,
                            xc);
    
    EXCEPTION
      WHEN OTHERS THEN
        NULL;

end prc_delete_all_apn_HLR;
/
