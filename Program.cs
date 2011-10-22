/*	BIC Functions : Performs commands on BIC files.
 * 
 * Date			Name		    Reason
 * ----------------------------------------------------------------------------------
 * 2007-02-15	KFW				Initial.
 * 2007-07-15	KFW				Added: SetBaseSkillRank(..).
 * 2007-07-16	KFW				Added more robust error checking for SetAbilityScore.
 * 2008-02-09	KFW				Added more robust error checking for all functions.
 * 2008-02-09	KFW				Added a Log function to record errors, can be used  
 *								by server admin for dedicated servers to track bugs!
 * 2008-10-11	KFW				Compiled for NWNX4 version 1.09!
 * 2008-10-11	KFW				Deprecated the GetBICFilename function.
 * 2008-10-11	KFW				Added the SetWing function.
 * 2008-10-11	KFW				Added the SetTail function.
 * 2008-10-12	KFW				Added the SetHairTint function.
 * 2008-10-12	KFW				Added the SetHeadTint function.
 * 2008-10-12	KFW				Added the SetBodyTint function.
 * 2009-08-06	FoamBats4All	Added the SetHead function.
 * 2009-08-06	FoamBats4All	Added the SetHair function.
 * 2009-08-13	FoamBats4All	Added the Retint function.
 * ---------------------------------------------------------------------------------- */

// Includes.
using System;
using System.Text;
using System.IO;
using OEIShared.IO.GFF;

namespace BICFunctions {
	class BICFunctionsMain {
		static void Main( string[] CommandLine ) {
			// Variables.
			string sError = "";

			// Invalid Command-line.
			// Param0 = Filename;
			// Param1 = Operation, Param2 = Operation Param1, Param3 = Operation Param 2.
			if ( ( CommandLine.Length - 1 ) % 3 != 0 ) {
				sError = "Error: Invalid command-line. Usage: BicFunctions.exe <Filename> <Operation1> <Parameter1> <Parameter2> <OperationN> <ParameterN1> <ParameterN2>";
				Log( sError );
				return;
			}

			// Open the specified BIC file.
			GFFFile BICFile;
			try {
				BICFile = new GFFFile( CommandLine[0] );
			} catch {
				sError = "Error: Cannot open specified filename, " + CommandLine[0] + " for reading/writing.";
				Log( sError );
				return;
			}

			// Cycle the command-line.
			uint uCommandIndex = 1, uCommandLength = (uint)CommandLine.Length - 1;
			for ( uCommandIndex = 1; uCommandIndex < uCommandLength + 1; uCommandIndex += 3 ) {
				if ( CommandLine[uCommandIndex] == "SetAbilityScore" ) SetAbilityScore( BICFile, CommandLine[uCommandIndex + 1], CommandLine[uCommandIndex + 2] );
				else if ( CommandLine[uCommandIndex] == "SetBaseSkillRank" ) SetBaseSkillRank( BICFile, CommandLine[uCommandIndex + 1], CommandLine[uCommandIndex + 2] );
				else if ( CommandLine[uCommandIndex] == "SetHead" ) SetHead( BICFile, CommandLine[uCommandIndex + 1] );
				else if ( CommandLine[uCommandIndex] == "SetHair" ) SetHair( BICFile, CommandLine[uCommandIndex + 1] );
				else if ( CommandLine[uCommandIndex] == "SetWing" ) SetWing( BICFile, CommandLine[uCommandIndex + 1] );
				else if ( CommandLine[uCommandIndex] == "SetTail" ) SetTail( BICFile, CommandLine[uCommandIndex + 1] );
				else if ( CommandLine[uCommandIndex] == "SetHairTint" ) SetHairTint( BICFile, CommandLine[uCommandIndex + 1] );
				else if ( CommandLine[uCommandIndex] == "SetHeadTint" ) SetHeadTint( BICFile, CommandLine[uCommandIndex + 1] );
				else if ( CommandLine[uCommandIndex] == "SetBodyTint" ) SetBodyTint( BICFile, CommandLine[uCommandIndex + 1] );
				else if ( CommandLine[uCommandIndex] == "Retint" ) Retint( BICFile );
				else Log( "Unknown command!" );
			}

			// Save the data modifications to the BIC file.
			BICFile.Save( CommandLine[0] );
			return;
		}

		// This function logs a message to BicFunctions.exe.txt.
		static void Log( string sMessage ) {
			// Broadcast the error.
			Console.WriteLine( sMessage );

			// Attempt to open the logfile for writing.
			TextWriter LogFile;
			try {
				LogFile = new StreamWriter( "BicFunctions.exe.txt" );
			} catch {
				Console.WriteLine( "Unable to write to logfile, BicFunctions.exe.txt. Check if it isn\'t being edited by another program." );
				return;
			}

			// Write the specified message to the logfile.
			LogFile.WriteLine( sMessage );
		}

		// This function parses a skill integer into a string name.
		static string ParseSkill( byte bSkill ) {
			switch ( bSkill ) {
				case 1: return ( "Concentration" );
				case 2: return ( "Disable trap" );
				case 3: return ( "Discipline" );
				case 4: return ( "Heal" );
				case 5: return ( "Hide" );
				case 6: return ( "Listen" );
				case 7: return ( "Lore" );
				case 8: return ( "Move silently" );
				case 9: return ( "Open lock" );
				case 10: return ( "Parry" );
				case 11: return ( "Perform" );
				case 12: return ( "Diplomacy" );
				case 13: return ( "Sleight of hand" );
				case 14: return ( "Search" );
				case 15: return ( "Set trap" );
				case 16: return ( "Spellcraft" );
				case 17: return ( "Spot" );
				case 18: return ( "Taunt" );
				case 19: return ( "Use magic device" );
				case 20: return ( "Appraise" );
				case 21: return ( "Tumble" );
				case 22: return ( "Craft trap" );
				case 23: return ( "Bluff" );
				case 24: return ( "Intimidate" );
				case 25: return ( "Craft armor" );
				case 26: return ( "Craft weapon" );
				case 27: return ( "Craft alchemy" );
				case 29: return ( "Survival" );
				default: return ( "Invalid skill" );
			}
		}

		// This function sets the character file's base skill rank.
		static void SetBaseSkillRank( GFFFile BICFile, string sSkill, string sRank ) {
			// Variables.
			byte bSkill = 0;
			byte bRank = 0;
			string sSkillName = ParseSkill( bSkill );
			string sError = "";

			// Filter: Skill is invalid.
			try {
				bSkill = Convert.ToByte( sSkill );
			} catch {
				sError = "Error: Skill value is invalid and must be an integer between 0 and 255.";
				Log( sError );
				return;
			}

			// Filter: Rank is invalid.
			try {
				bRank = Convert.ToByte( sRank );
			} catch {
				sError = "Error: Rank value is invalid and must be an integer between 0 and 255.";
				Log( sError );
				return;
			}

			// Get the skills list.
			if ( BICFile.TopLevelStruct.Contains( "SkillList" ) ) {
				GFFList Skills = BICFile.TopLevelStruct["SkillList"].ValueList;
				//Get the required skill.
				GFFStruct Skill;
				try {
					Skill = Skills[bSkill];
				} catch {
					sError = "Error: SetBaseSkillRank( " + sSkillName + ", " + sRank + " ): Unknown skill. Please check your command-line and the skills.2da file for the skill integers.";
					Log( sError );
					return;
				}
				if ( Skill.Contains( "Rank" ) )
					// Modify it.
					BICFile.TopLevelStruct["SkillList"].ValueList[bSkill]["Rank"].ValueByte = bRank;
				else {
					sError = "Error: SetBaseSkillRank( " + sSkillName + ", " + sRank + " ): Skill entry corrupted, its missing a byte entry called Rank. Please check with a BIC editor.";
					Log( sError );
				}
			} else {
				sError = "Error: SetBaseSkillRank( " + sSkillName + ", " + sRank + " ): Skills list not found. Possible invalid BIC file. Please check with a BIC editor.";
				Log( sError );
			}
			return;
		}

		// This function sets the character file's ability score.
		static void SetAbilityScore( GFFFile BICFile, string sAbility, string sScore ) {
			// Variables.
			byte bScore = 0;
			string sError = "";

			// Filter: Rank is invalid or exceeds byte size.
			try {
				bScore = Convert.ToByte( sScore );
			} catch {
				sError = "Error: Score value is invalid and must be an integer between 0 and 255.";
				Console.WriteLine( sError );
				Log( sError );
				return;
			}

			// Ability score exists, modify it.
			if ( BICFile.TopLevelStruct.Contains( sAbility ) ) {
				try {
					BICFile.TopLevelStruct[sAbility].ValueByte = bScore;
				} catch {
					sError = "Error: SetAbilityScore( " + sAbility + ", " + sScore + " ): Can't update ability. Please check your command-line. The acceptable abilities are as follows: Str, Dex, Con, Int, Cha, Wis. And the score value should be an integer between 0 and 255 all inclusive.";
					Log( sError );
					return;
				}
			} else {
				sError = "Error: SetAbilityScore( " + sAbility + ", " + sScore + " ): Ability not found. Possible typographical error. Please check with a BIC editor.";
				Log( sError );
			}
			return;
		}

		// This function sets the character file's head ID.
		static void SetHead( GFFFile BICFile, string sHeadId ) {
			// Variables.
			byte bHeadID = 0;
			string sError = "";

			// Filter: Hair ID is invalid or exceeds byte size.
			try {
				bHeadID = Convert.ToByte( sHeadId );
			} catch {
				sError = "Error: Head ID type is invalid and must be an integer between 0 and 255.";
				Log( sError );
				return;
			}

			// If Hair ID exists, modify it.
			try {
				BICFile.TopLevelStruct["Appearance_Head"].ValueByte = bHeadID;
			} catch {
				sError = "Error: SetHead( " + sHeadId + " ) : Can't update head ID. Please check your command-line. The ID should be an integer between 0 and 255 all inclusive.";
				Log( sError );
				return;
			}
			return;
		}

		// This function sets the character file's hair ID.
		static void SetHair( GFFFile BICFile, string sHairId ) {
			// Variables.
			byte bHairID = 0;
			string sError = "";

			// Filter: Hair ID is invalid or exceeds byte size.
			try {
				bHairID = Convert.ToByte( sHairId );
			} catch {
				sError = "Error: Head ID type is invalid and must be an integer between 0 and 255.";
				Log( sError );
				return;
			}

			// If Hair ID exists, modify it.
			try {
				BICFile.TopLevelStruct["Appearance_Hair"].ValueByte = bHairID;
			} catch {
				sError = "Error: SetHair( " + sHairId + " ) : Can't update hair ID. Please check your command-line. The ID should be an integer between 0 and 255 all inclusive.";
				Log( sError );
				return;
			}

			Retint( BICFile );
			return;
		}

		// This function sets the character file's wing type.
		static void SetWing( GFFFile BICFile, string sWingType ) {
			// Variables.
			byte bWingType = 0;
			string sError = "";

			// Filter: Wing type is invalid or exceeds byte size.
			try {
				bWingType = Convert.ToByte( sWingType );
			} catch {
				sError = "Error: Wing type is invalid and must be an integer between 0 and 255.";
				Log( sError );
				return;
			}

			// If Wing type exists, modify it.
			try {
				BICFile.TopLevelStruct["Wings"].ValueByte = bWingType;
			} catch {
				sError = "Error: SetWing( " + sWingType + " ) : Can't update wing type. Please check your command-line. And the wing type should be an integer between 0 and 255 all inclusive.";
				Log( sError );
				return;
			}
			Retint( BICFile );
			return;
		}

		// This function sets the character file's tail type.
		static void SetTail( GFFFile BICFile, string sTailType ) {
			// Variables.
			byte bTailType = 0;
			string sError = "";

			// Filter: Tail type is invalid or exceeds byte size.
			try {
				bTailType = Convert.ToByte( sTailType );
			} catch {
				sError = "Error: Tail type is invalid and must be an integer between 0 and 255.";
				Log( sError );
				return;
			}

			// If Tail type exists, modify it.
			try {
				BICFile.TopLevelStruct["Tail"].ValueByte = bTailType;
			} catch {
				sError = "Error: SetTail( " + sTailType + " ) : Can't update tail type. Please check your command-line. And the wing type should be an integer between 0 and 255 all inclusive.";
				Log( sError );
				return;
			}

			Retint( BICFile );
			return;
		}

		// This function sets the character file's hair colour.
		static void SetHairTint( GFFFile BICFile, string sTint ) {
			// Variables.
			byte bTint1r = 0, bTint1g = 0, bTint1b = 0;
			byte bTint2r = 0, bTint2g = 0, bTint2b = 0;
			byte bTint3r = 0, bTint3g = 0, bTint3b = 0;
			string sError = "";

			// Filter: Hair colour string values are invalid.
			try {
				bTint1r = Convert.ToByte( sTint.Substring( 0, 3 ) );
				bTint1g = Convert.ToByte( sTint.Substring( 3, 3 ) );
				bTint1b = Convert.ToByte( sTint.Substring( 6, 3 ) );
				bTint2r = Convert.ToByte( sTint.Substring( 9, 3 ) );
				bTint2g = Convert.ToByte( sTint.Substring( 12, 3 ) );
				bTint2b = Convert.ToByte( sTint.Substring( 15, 3 ) );
				bTint3r = Convert.ToByte( sTint.Substring( 18, 3 ) );
				bTint3g = Convert.ToByte( sTint.Substring( 21, 3 ) );
				bTint3b = Convert.ToByte( sTint.Substring( 24, 3 ) );
			} catch {
				sError = "Error: Hair colour input strings are invalid and must be string integers between 0 and 255.";
				Log( sError );
				return;
			}

			// If the Tint_Hair structure exists, modify it.
			try {
				GFFStruct gffTintHair1 = BICFile.TopLevelStruct.GetStructSafe( "Tint_Hair" ).GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "1" );
				GFFStruct gffTintHair2 = BICFile.TopLevelStruct.GetStructSafe( "Tint_Hair" ).GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "2" );
				GFFStruct gffTintHair3 = BICFile.TopLevelStruct.GetStructSafe( "Tint_Hair" ).GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "3" );
				gffTintHair1["r"].ValueByte = bTint1r;
				gffTintHair1["g"].ValueByte = bTint1g;
				gffTintHair1["b"].ValueByte = bTint1b;
				gffTintHair2["r"].ValueByte = bTint2r;
				gffTintHair2["g"].ValueByte = bTint2g;
				gffTintHair2["b"].ValueByte = bTint2b;
				gffTintHair3["r"].ValueByte = bTint3r;
				gffTintHair3["g"].ValueByte = bTint3g;
				gffTintHair3["b"].ValueByte = bTint3b;
			} catch {
				sError = "Error: SetHairTint( " + sTint + " ) : Can't update hair tint. Please check your command-line.";
				Log( sError );
				return;
			}

			return;
		}

		// This function sets the character file's head colour.
		static void SetHeadTint( GFFFile BICFile, string sTint ) {
			// Variables.
			byte bTint1r = 0, bTint1g = 0, bTint1b = 0;
			byte bTint2r = 0, bTint2g = 0, bTint2b = 0;
			byte bTint3r = 0, bTint3g = 0, bTint3b = 0;
			string sError = "";

			// Filter: Head colour string values are invalid.
			try {
				bTint1r = Convert.ToByte( sTint.Substring( 0, 3 ) );
				bTint1g = Convert.ToByte( sTint.Substring( 3, 3 ) );
				bTint1b = Convert.ToByte( sTint.Substring( 6, 3 ) );
				bTint2r = Convert.ToByte( sTint.Substring( 9, 3 ) );
				bTint2g = Convert.ToByte( sTint.Substring( 12, 3 ) );
				bTint2b = Convert.ToByte( sTint.Substring( 15, 3 ) );
				bTint3r = Convert.ToByte( sTint.Substring( 18, 3 ) );
				bTint3g = Convert.ToByte( sTint.Substring( 21, 3 ) );
				bTint3b = Convert.ToByte( sTint.Substring( 24, 3 ) );
			} catch {
				sError = "Error: Head colour input strings are invalid and must be string integers between 0 and 255.";
				Log( sError );
				return;
			}

			// If the Tint_Head structure exists, modify it.
			try {
				GFFStruct gffTintHead1 = BICFile.TopLevelStruct.GetStructSafe( "Tint_Head" ).GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "1" );
				GFFStruct gffTintHead2 = BICFile.TopLevelStruct.GetStructSafe( "Tint_Head" ).GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "2" );
				GFFStruct gffTintHead3 = BICFile.TopLevelStruct.GetStructSafe( "Tint_Head" ).GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "3" );
				gffTintHead1["r"].ValueByte = bTint1r;
				gffTintHead1["g"].ValueByte = bTint1g;
				gffTintHead1["b"].ValueByte = bTint1b;
				gffTintHead2["r"].ValueByte = bTint2r;
				gffTintHead2["g"].ValueByte = bTint2g;
				gffTintHead2["b"].ValueByte = bTint2b;
				gffTintHead3["r"].ValueByte = bTint3r;
				gffTintHead3["g"].ValueByte = bTint3g;
				gffTintHead3["b"].ValueByte = bTint3b;
			} catch {
				sError = "Error: SetHeadTint( " + sTint + " ) : Can't update head tint. Please check your command-line.";
				Log( sError );
				return;
			}
			return;
		}

		// This function sets the character file's body colour.
		static void SetBodyTint( GFFFile BICFile, string sTint ) {
			// Variables.
			byte bTint1a = 0, bTint1r = 0, bTint1g = 0, bTint1b = 0;
			byte bTint2a = 0, bTint2r = 0, bTint2g = 0, bTint2b = 0;
			byte bTint3a = 0, bTint3r = 0, bTint3g = 0, bTint3b = 0;
			string sError = "";

			// Filter: Body colour string values are invalid.
			try {
				bTint1r = Convert.ToByte( sTint.Substring( 0, 3 ) );
				bTint1g = Convert.ToByte( sTint.Substring( 3, 3 ) );
				bTint1b = Convert.ToByte( sTint.Substring( 6, 3 ) );
				bTint2r = Convert.ToByte( sTint.Substring( 9, 3 ) );
				bTint2g = Convert.ToByte( sTint.Substring( 12, 3 ) );
				bTint2b = Convert.ToByte( sTint.Substring( 15, 3 ) );
				bTint3r = Convert.ToByte( sTint.Substring( 18, 3 ) );
				bTint3g = Convert.ToByte( sTint.Substring( 21, 3 ) );
				bTint3b = Convert.ToByte( sTint.Substring( 24, 3 ) );
			} catch {
				sError = "Error: Head colour input strings are invalid and must be string integers between 0 and 255.";
				Log( sError );
				return;
			}

			// If the Tintable structure exists, modify it.
			try {
				GFFStruct gffTintBody1 = BICFile.TopLevelStruct.GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "1" );
				GFFStruct gffTintBody2 = BICFile.TopLevelStruct.GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "2" );
				GFFStruct gffTintBody3 = BICFile.TopLevelStruct.GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "3" );
				gffTintBody1["a"].ValueByte = bTint1a; Console.WriteLine( bTint1a );
				gffTintBody1["r"].ValueByte = bTint1r; Console.WriteLine( bTint1r );
				gffTintBody1["g"].ValueByte = bTint1g; Console.WriteLine( bTint1g );
				gffTintBody1["b"].ValueByte = bTint1b; Console.WriteLine( bTint1b );
				gffTintBody2["a"].ValueByte = bTint2a; Console.WriteLine( bTint2a );
				gffTintBody2["r"].ValueByte = bTint2r; Console.WriteLine( bTint2r );
				gffTintBody2["g"].ValueByte = bTint2g; Console.WriteLine( bTint2g );
				gffTintBody2["b"].ValueByte = bTint2b; Console.WriteLine( bTint2b );
				gffTintBody3["a"].ValueByte = bTint3a; Console.WriteLine( bTint3a );
				gffTintBody3["r"].ValueByte = bTint3r; Console.WriteLine( bTint3r );
				gffTintBody3["g"].ValueByte = bTint3g; Console.WriteLine( bTint3g );
				gffTintBody3["b"].ValueByte = bTint3b; Console.WriteLine( bTint3b );
			} catch {
				sError = "Error: SetBodyTint( " + sTint + " ) : Can't update body tint. Please check your command-line.";
				Log( sError );
				return;
			}
			return;
		}

		// This function sets the character file's body colour based on their head color.
		static void ResetBodyTint( GFFFile BICFile ) {
			// Variables.
			byte bTintA = 0, bTintR = 0, bTintG = 0, bTintB = 0;
			string sError = "";

			// Filter: Body colour string values are invalid.
			try {
				GFFStruct gffTintBody = BICFile.TopLevelStruct.GetStructSafe( "Tint_Head" ).GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "1" );
				bTintA = gffTintBody["a"].ValueByte;
				bTintR = gffTintBody["r"].ValueByte;
				bTintG = gffTintBody["g"].ValueByte;
				bTintB = gffTintBody["b"].ValueByte;
			} catch {
				sError = "Error: Head colour could not be gained.";
				Log( sError );
				return;
			}

			// If the Tintable structure exists, modify it.
			try {
				GFFStruct gffTintBody1 = BICFile.TopLevelStruct.GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "1" );
				GFFStruct gffTintBody2 = BICFile.TopLevelStruct.GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "2" );
				GFFStruct gffTintBody3 = BICFile.TopLevelStruct.GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "3" );
				gffTintBody1["a"].ValueByte = bTintA; Console.WriteLine( bTintA );
				gffTintBody1["r"].ValueByte = bTintR; Console.WriteLine( bTintR );
				gffTintBody1["g"].ValueByte = bTintG; Console.WriteLine( bTintG );
				gffTintBody1["b"].ValueByte = bTintB; Console.WriteLine( bTintB );
				gffTintBody2["a"].ValueByte = bTintA; Console.WriteLine( bTintA );
				gffTintBody2["r"].ValueByte = bTintR; Console.WriteLine( bTintR );
				gffTintBody2["g"].ValueByte = bTintG; Console.WriteLine( bTintG );
				gffTintBody2["b"].ValueByte = bTintB; Console.WriteLine( bTintB );
				gffTintBody3["a"].ValueByte = bTintA; Console.WriteLine( bTintA );
				gffTintBody3["r"].ValueByte = bTintR; Console.WriteLine( bTintR );
				gffTintBody3["g"].ValueByte = bTintG; Console.WriteLine( bTintG );
				gffTintBody3["b"].ValueByte = bTintB; Console.WriteLine( bTintB );
			} catch {
				sError = "Error: ResetBodyTint() : Can't update body tint.";
				Log( sError );
				return;
			}
			return;
		}

		// This function sets the character file's body colour based on head color.
		static void Retint( GFFFile BICFile ) {
			// Variables.
			byte bTintA = 0, bTintR = 0, bTintG = 0, bTintB = 0;
			string sError = "";

			// Filter: Body colour string values are invalid.
			try {
				GFFStruct gffTintBody = BICFile.TopLevelStruct.GetStructSafe( "Tint_Head" ).GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "1" );
				bTintR = Convert.ToByte( gffTintBody["r"].Value );
				bTintG = Convert.ToByte( gffTintBody["g"].Value );
				bTintB = Convert.ToByte( gffTintBody["b"].Value );
			} catch {
				sError = "Error: Head colour input strings are invalid and must be string integers between 0 and 255.";
				Log( sError );
				return;
			}

			// If the Tintable structure exists, modify it.
			try {
				GFFStruct gffTintBody1 = BICFile.TopLevelStruct.GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "1" );
				GFFStruct gffTintBody2 = BICFile.TopLevelStruct.GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "2" );
				GFFStruct gffTintBody3 = BICFile.TopLevelStruct.GetStructSafe( "Tintable" ).GetStructSafe( "Tint" ).GetStructSafe( "3" );
				gffTintBody1["a"].ValueByte = bTintA; Console.WriteLine( bTintA );
				gffTintBody1["r"].ValueByte = bTintR; Console.WriteLine( bTintR );
				gffTintBody1["g"].ValueByte = bTintG; Console.WriteLine( bTintG );
				gffTintBody1["b"].ValueByte = bTintB; Console.WriteLine( bTintB );
				gffTintBody2["a"].ValueByte = bTintA; Console.WriteLine( bTintA );
				gffTintBody2["r"].ValueByte = bTintR; Console.WriteLine( bTintR );
				gffTintBody2["g"].ValueByte = bTintG; Console.WriteLine( bTintG );
				gffTintBody2["b"].ValueByte = bTintB; Console.WriteLine( bTintB );
				gffTintBody3["a"].ValueByte = bTintA; Console.WriteLine( bTintA );
				gffTintBody3["r"].ValueByte = bTintR; Console.WriteLine( bTintR );
				gffTintBody3["g"].ValueByte = bTintG; Console.WriteLine( bTintG );
				gffTintBody3["b"].ValueByte = bTintB; Console.WriteLine( bTintB );
			} catch {
				sError = "Error: Retint : Can't update body tint. Please check your command-line.";
				Log( sError );
				return;
			}
			return;
		}
	}
}