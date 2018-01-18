using System;

namespace FishEyeClient.Entities
{
	/**** response example *****
	 *
	 <fileRevision
	      totalLines="88"
	      fileRevisionState="CHANGED"
	     rev="143520"
	     path="Base/branches/7.8.0/SqlScripts/VwSysProcessLogOracle/VwSysProcessLogOracle.sql"
	     linesRemoved="0"
	     linesAdded="13"
	     date="2017-12-01T12:36:22.738+02:00"
	     csid="143520"
	     contentLink="/browse/~raw,r=143520/PackageStore/Base/branches/7.8.0/SqlScripts/VwSysProcessLogOracle/VwSysProcessLogOracle.sql"
	     author="m.borovyk">
	   <ancestor>143395</ancestor>
	   <comment>
	      #CRM-34805 Идентификация экземпляров процесса которые запускались в режиме отладки
	   </comment>
	 </fileRevision>
	 *
	 */
	public class FileRevision
	{
		public int TotalLines { get; set; }
		public string FileRevisionState { get; set; }
		public string Rev { get; set; }
		public string Path { get; set; }
		public int LinesRemoved { get; set; }
		public int LinesAdded { get; set; }
		public DateTime Date { get; set; }
		public string Csid { get; set; }
		public string Author { get; set; }
		public string Comment { get; set; }

	}
}
