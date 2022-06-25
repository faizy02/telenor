create or replace procedure onlinecreate_hlr(msisdn in varchar2) is
                                             --status out varchar2
                                             
  PRAGMA AUTONOMOUS_TRANSACTION;
  T_MSISDN VARCHAR2(50);
  tIMSI    varchar2(15);
  --scount   number;
  --soidN    varchar2(20);
  tid      varchar2(30);
  xc       clob;
  tranID   number;
  SourceID varchar2(10);
  T_XML    VARCHAR2(2000);
  --T_STR VARCHAR2(1000);
  T_XML_AUTH    VARCHAR2(2000);
  T_XML_USER    VARCHAR2(2000);
  --status VARCHAR2(50);
  T_IMSI VARCHAR2(15);
  T_SIM_KEY number;
  T_KI_VALUE VARCHAR(200);
  T_MSISDN_REGION VARCHAR2(10);
  T_PROFILE number;
begin

  T_MSISDN := msisdn;
  provident_loading(T_MSISDN, '');

  --Del user:msisdn=xxxxxx,DelAuth=1;

  SourceID := 'QHLR';
  SELECT seqnocss.nextval INTO tranID FROM dual;

  --select start_range into tIMSI from zterouting where ne_id='ZteHlr3' and rownum<2;
  tIMSI := '410060000000001';
  T_XML := '<ZTEDirectMML><IMSI>' || tIMSI ||'</IMSI><DirectMML>"DEL USER : MSISDN=' || '92' ||substr(trim(T_MSISDN), -10, 10) ||',DelAuth=1;"</DirectMML></ZTEDirectMML>';

  tid := SourceID || '+' || to_char(tranID);
  oraclesanoc.putrequest(tranID,
                         SourceID,
                         '85',
                         '92' || substr(trim(msisdn), -10, 10),
                         tIMSI,
                         '',
                         T_XML,
                         xc);
                         
  dbms_output.put_line(tid);
  dbms_lock.sleep(1); 
  ------------------------- WAIT FOR TERMINATION ----------------------------------
  
  /*select soid into SoiDN from sorecord where transactionID = tid;
  
  select substr(dbms_lob.substr(e.logtext, 4000, 1),
                instr(dbms_lob.substr(e.logtext, 4000, 1), 'IMSI') + 5,
                15)
    into imsi
    from eptlog e
   where soid = soidn;

  if (imsi like '41%') then
    dbms_lock.sleep(1);
    status := 'Success';
  else
    imsi := 'Failed';
    status := 'Failed';
  end if;*/
------------------ GET DATA FROM PROVIDENT TO SEND TO HLR FOR CREATION --------------
select t.parametervalue into T_SIM_KEY from serviceparameter t where t.subscriberkey in (select sp.subscriberkey from serviceparameter sp where parametervalue in (T_MSISDN)) and t.parametername = 'SIM_TRANSKEY';
select t.parametervalue into T_IMSI from serviceparameter t where t.subscriberkey in (select sp.subscriberkey from serviceparameter sp where parametervalue in (T_MSISDN)) and t.parametername = 'GSM_IMSI1';
select t.parametervalue into T_KI_VALUE from serviceparameter t where t.subscriberkey in (select sp.subscriberkey from serviceparameter sp where parametervalue in (T_MSISDN)) and t.parametername = 'GSM_ENCKEY';
select t.region_1 into T_MSISDN_REGION from hlrrouting_s t where t.start_range <= T_MSISDN and t.end_range >= T_MSISDN;

if (T_SIM_KEY in (21,22,23)) then
   T_XML_AUTH :=  '<ZTEDirectMML><IMSI>' || tIMSI ||'</IMSI><DirectMML>"ADD AUTH: IMSI=' || T_IMSI || ',SecVer=20,KI=' || T_KI_VALUE  || ',AMF=8234, AKFG=1, reSynFg=1, OVID=' || T_SIM_KEY || ', KEYID=' || T_SIM_KEY ||';"</DirectMML></ZTEDirectMML>';
   if (T_MSISDN_REGION = 'North') then T_PROFILE := 134; end if;
   if (T_MSISDN_REGION = 'Central') then T_PROFILE := 135; end if;
   if (T_MSISDN_REGION = 'South') then T_PROFILE := 136; end if;
   T_XML_USER :=  '<ZTEDirectMML><IMSI>' || tIMSI ||'</IMSI><DirectMML>"ADD USER: IMSI=' || T_IMSI || ', MSISDN=' || '92' ||substr(trim(T_MSISDN), -10, 10)  || ', PROFILE=' || T_PROFILE ||';"</DirectMML></ZTEDirectMML>';
else
   T_XML_AUTH :=  '<ZTEDirectMML><IMSI>' || tIMSI ||'</IMSI><DirectMML>"ADD AUTH: IMSI=' || T_IMSI || ',KI=' || T_KI_VALUE || ', KEYID=' || T_SIM_KEY ||',SecVer=1;"</DirectMML></ZTEDirectMML>';
   if (T_MSISDN_REGION = 'North') then T_PROFILE := 124; end if;
   if (T_MSISDN_REGION = 'Central') then T_PROFILE := 125; end if;
   if (T_MSISDN_REGION = 'South') then T_PROFILE := 126; end if;
   T_XML_USER :=  '<ZTEDirectMML><IMSI>' || tIMSI ||'</IMSI><DirectMML>"ADD USER: IMSI=' || T_IMSI || ', MSISDN=' || '92' ||substr(trim(T_MSISDN), -10, 10)  || ', PROFILE=' || T_PROFILE ||';"</DirectMML></ZTEDirectMML>';

end if;

--------- SEND EPT ADD AUTH THEN ADD USER TO HLR -----------------
  SELECT seqnocss.nextval INTO tranID FROM dual;
  oraclesanoc.putrequest(tranID,
                         SourceID,
                         '85',
                         '92' || substr(trim(msisdn), -10, 10),
                         tIMSI,
                         '',
                         T_XML_AUTH,
                         xc);
  dbms_lock.sleep(1);
  SELECT seqnocss.nextval INTO tranID FROM dual;                       
  oraclesanoc.putrequest(tranID,
                         SourceID,
                         '85',
                         '92' || substr(trim(msisdn), -10, 10),
                         tIMSI,
                         '',
                         T_XML_USER,
                         xc);                       
INSERT INTO online_creation_hlr_log VALUES(T_MSISDN,T_IMSI,T_KI_VALUE,'Success',sysdate);
commit;
end onlinecreate_hlr;
/
