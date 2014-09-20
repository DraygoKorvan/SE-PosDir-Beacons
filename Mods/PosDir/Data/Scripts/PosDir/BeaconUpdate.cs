namespace PosDirBeaconsSource
{
	using Sandbox.Common;
	using Sandbox.Common.Components;
	using Sandbox.Common.ObjectBuilders;
	using System;

	/// <summary>
	/// Adds logic to the Beacon cube, allowing the Beacon to broadcast its exact position in the sector.
	/// Has been tested for ships that transition to station and back again.
	/// Author: Midspace, Draygo
	/// </summary>
	/// <example>
	/// To use, simply add the proper key to the very start of the Beacon Name field.
	/// For Position: "Pos:"
	/// For ship Direction: "Dir:"
	/// </example>
	/// <see cref="http://steamcommunity.com/id/ScreamingAngels/myworkshopfiles/?appid=244850"/>
    /// <see cref="http://steamcommunity.com/id/DraygoKorvan/myworkshopfiles/?appid=244850"/>
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_Beacon))]
	public class PosDirBeaconLogic : MyGameLogicComponent
	{
		private const string CustomNameKeyPos = "Pos:";
		private const string CustomNameKeyDir = "Dir:";

		public override void Close()
		{
		}

		public override void Init(Sandbox.Common.ObjectBuilders.MyObjectBuilder_EntityBase objectBuilder)
		{
			Entity.NeedsUpdate |= MyEntityUpdateEnum.EACH_10TH_FRAME;
			
		}

		public override void MarkForClose()
		{
		}

		public override void UpdateAfterSimulation()
		{

		}

		public override void UpdateAfterSimulation10()
		{
			try
			{
				var beacon = (Sandbox.ModAPI.Ingame.IMyTerminalBlock)Entity;
				if (beacon.CustomName != null && beacon.CustomName.StartsWith(CustomNameKeyPos, StringComparison.InvariantCultureIgnoreCase))
				{
					var pos = beacon.CubeGrid.GridIntegerToWorld(beacon.Position);
					beacon.SetCustomName(string.Format("{0} [X:{1:N0}  Y:{2:N0}  Z:{3:N0}]", CustomNameKeyPos, (int)pos.X, (int)pos.Y, (int)pos.Z));
				}
				else if (beacon.CustomName != null && beacon.CustomName.StartsWith(CustomNameKeyDir, StringComparison.InvariantCultureIgnoreCase))
				{
					var orientation = beacon.WorldMatrix.Up;
					beacon.SetCustomName(string.Format("{0} [X:{1:N5}  Y:{2:N5} Z:{3:N5}]", CustomNameKeyDir, orientation.X, orientation.Y, orientation.Z));
				}
			}
			catch (Exception)
			{

			}

		}

		public override void UpdateAfterSimulation100()
		{
		}

		public override void UpdateBeforeSimulation()
		{
		}

		public override void UpdateBeforeSimulation10()
		{
		}

		public override void UpdateBeforeSimulation100()
		{
		}

		public override void UpdateOnceBeforeFrame()
		{
		}
	}

}
