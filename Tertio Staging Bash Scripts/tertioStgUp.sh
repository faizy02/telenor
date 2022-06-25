#!/bin/bash

function clusterstart {

  echo "Starting Cluster $1"

  Cluster start $1
  while :
  do
    sleep 2s
    clusterstatus="$(Cluster list | grep $1)"
    if (echo "$clusterstatus" | grep -q "NORMAL"); then
        echo "$1 Started";
        echo "----------------";
        break;
    else
        echo "$1 is still in starting phase";
    fi
  done
}

function tertio_up {
  echo "TERTIO STARTING UP"
  cd /home/app/tertio7_6/bin
  sh ./provenv

  echo "---------------------"
  echo "---Starting Server---"
  Cluster start tertio.server
  sleep 3s
  
  clusterstart tertio.core
  clusterstart tertio.stps.20a
  clusterstart tertio.stps.20b
  clusterstart tertio.PA_CBS1
  clusterstart tertio.PA_CBS
  clusterstart tertio.PA_CC
  clusterstart tertio.PA_HLRa
  clusterstart tertio.PA_general
  clusterstart tertio.PA_internal
  clusterstart tertio.SA_BSA
  clusterstart tertio.SA_EAI
  clusterstart tertio.SA_ORACLE
  
  echo "---Starting Content Connector---"
  CCStart
  wait
  
    
  for i in `ps -ef |grep rvd| awk '{ print $2 }'`; 
  do 
    kill -9 $i; 
  done
  
  su -c '/home/tertiodau1/tibco_rvd/tibrv/8.3/bin/rvd -listen tcp:7477' tertiodau1
}



# Warning menu for user to whether quit or continue
Warning='Do you want to continue START UP TERTIO: '
options=("yes" "no")
select opt in "${options[@]}"
  do
      case $opt in
          "yes")
              echo "you chose Yes"
              tertio_up
		break;
              ;;
          "no")
              echo "you chose No"
              echo "Script Not Executed!"
              break;
              ;;
          *) echo invalid option;;
      esac
  done
