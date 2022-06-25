create or replace procedure number_bar(msisdn in number) is
 /* 
  -- Syed Faizan Sajjad, 11Apr16
  -- This procedure is used for barring number on HLR
  -- The transaction id for these requests will start from number_bar
  --**********************************************************************************************************************/
  imsi    varchar2(15);
  xml     clob;
  --transc  varchar2(27);
  xc      varchar2(20);
  msi varchar2(11);
  tid varchar2(30);
  tranID   number;
  SourceID varchar2(10);
  
begin
  
  imsi := '';
  xml := '';
  msi:='0'||msisdn;
----------- Getting Imsi
    select u.subscriberkey into imsi
    from serviceparameter u
    where u.parametervalue = msi
    and u.parametername='GSM_MSISDN';
    
----------- 
    
    --xml:='<ZTEDirectMML><IMSI>'||imsi||'</IMSI><DirectMML>"Mod BscEx:MSISDN='||'92'||substr(msisdn,-10,10)||',BIC=1,BOC=1,BPOS=1;"</DirectMML></ZTEDirectMML>';
    
    select TEMP.Nextval into tranID from dual;
    
    SourceID := 'NBAR';    
    tid := SourceID || '+' || to_char(tranID);
    
    oraclesanoc.putrequest(tranID,SourceID,'85','92' || substr(trim(msisdn), -10, 10),imsi,'','<ZTEDirectMML><IMSI>'||imsi||'</IMSI><DirectMML>"Mod BscEx:MSISDN='||'92'||substr(trim(msisdn),-10,10)||',BIC=1,BOC=1,BPOS=1;"</DirectMML></ZTEDirectMML>',xc);

end number_bar;
/
