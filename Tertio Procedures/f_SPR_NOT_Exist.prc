create or replace procedure f_SPR_NOT_Exist is

  IMSI varchar2(15);

  --  Cursor SME is

begin

  for SMELoop in (select *
                    from eai_errors
                   where flagerror = '1'
                     and aplication in ( 'Provident')
                     and errorsistdesc like ('%131501%')
                     --and subject not like ('%PrePosMigration%') 
                     and rownum<16) loop
    
    
    if (SMELoop.Msisdn is NULL) then
      SMELoop.Msisdn := SMELoop.Customerid;
    end if;
    
    DBMS_OUTPUT.PUT_LINE(SMELoop.Msisdn || ' EAI_ERROR_ID: ' ||
                         SMELoop.Eai_Error_Id || 'FlagError: ' || SMELoop.Flagerror);
    
    --GETTING IMSI FROM PROVIDENT
    select subscriberkey
      into IMSI
      from tertiodau1.serviceparameter@dbl_provda
     where parametervalue = SMELoop.Msisdn
     and rownum<2;

    --DELETING A442 FROM PROVIDENT
    
    --if (SMELoop.Subject not like ('%PrePosMigration%')) then
    delete from tertiodau1.subscriberservice@dbl_provda
     where subscriberkey = IMSI
       and servicename = 'A442';
    commit;
     
    
    --Logging in the Log Table

    --Add A442 into Provident using NOC services table so that request is sent to HLR.
    INSERT INTO tertiodau1.t_NOCSS_All_Services@dbl_provda
    Values
      (SMELoop.Msisdn, 'A-442', 'A', '02', null);
    commit;
    
    --Executing procedure sending request to HLR for A442 addition
    tertiodau1.Prc_NOCSS_All_Services@dbl_provda();

    --REPROCESS ERROR FROM ERROR CONSOLE
    update eai_errors set isresending='1' where eai_error_id=SMELoop.Eai_Error_Id;
    commit;
           dbms_lock.sleep(3);
          DBMS_OUTPUT.PUT_LINE(SMELoop.Msisdn || ' EAI_ERROR_ID: ' ||
                           SMELoop.Eai_Error_Id || 'FlagError: ' || SMELoop.Flagerror);

  end loop;
end f_SPR_NOT_Exist;
/
