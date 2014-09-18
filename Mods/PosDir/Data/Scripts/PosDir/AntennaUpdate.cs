using VRageMath;

namespace PosDirBeaconsSource
{
	using Sandbox.Common;
	using Sandbox.Common.Components;
	using Sandbox.Common.ObjectBuilders;
	using Sandbox.ModAPI;
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
	/// For Nearest Asteroid Direction: "Probe:"
	/// </example>
	/// <see cref="http://steamcommunity.com/id/ScreamingAngels/myworkshopfiles/?appid=244850"/>
	/// <see cref="http://steamcommunity.com/id/DraygoKorvan/myworkshopfiles/?appid=244850"/>
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_RadioAntenna))]
	public class PosDirAntennaLogic : MyGameLogicComponent
	{
		private const string CustomNameKeyPos = "Pos:";
		private const string CustomNameKeyDir = "Dir:";
		private const string CustomNameKeyProbe = "Probe:";

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
				var antenna = (Sandbox.ModAPI.Ingame.IMyTerminalBlock)Entity;
				var entityBlock = (IMyCubeBlock)Entity;

				// called multiple times for ships, to be kept up to date.
				if (antenna.CustomName != null && antenna.CustomName.StartsWith(CustomNameKeyPos, StringComparison.InvariantCultureIgnoreCase))
				{
					var pos = antenna.CubeGrid.GridIntegerToWorld(antenna.Position);
					//dont really care about the decimal places so cast as int. 
					antenna.SetCustomName(string.Format("{0} [X:{1:N}  Y:{2:N}  Z:{3:N}]", CustomNameKeyPos, (int)(pos.X), (int)(pos.Y), (int)(pos.Z)));
				}
				else if (antenna.CustomName != null && antenna.CustomName.StartsWith(CustomNameKeyDir, StringComparison.InvariantCultureIgnoreCase))
				{
					var orientation = antenna.GetTopMostParent().WorldMatrix.Forward;
					//care about the decimal places as being off a few degrees can be very significant. vector3.tostring is already localized. 
					antenna.SetCustomName(string.Format("{0} {1}", CustomNameKeyDir, orientation.ToString()));
				}
				else if (antenna.CustomName != null && antenna.CustomName.StartsWith(CustomNameKeyProbe, StringComparison.InvariantCultureIgnoreCase))
				{
					antenna.SetCustomName(CustomNameKeyProbe);
					var pos = antenna.CubeGrid.GridIntegerToWorld(antenna.Position);
					
					Vector3 closestpos = Vector3.Zero;
					float closestdist = float.MaxValue;
					int i = 0;
					IMyVoxelMap map = null;
					for (i=0;  (map = MyAPIGateway.Session.VoxelMaps.GetVoxelMap(i)) != null; i++)
					{
						var mappos = map.PositionLeftBottomCorner + map.SizeInMetresHalf;
						if (Vector3.Distance(mappos, pos) < closestdist)
						{
							closestpos = mappos;
							closestdist = Vector3.Distance(mappos, pos);
						}
					}
					if (closestpos != Vector3.Zero)
					{
						var direction = closestpos - pos;
						direction = Vector3.Normalize(direction);
						
						var obj = (MyObjectBuilder_RadioAntenna)entityBlock.GetObjectBuilderCubeBlock();
						if (closestdist < 1000 && obj.BroadcastRadius > closestdist)
							antenna.SetCustomName(string.Format("{0} Asteroid too close. Distance: {1:N}", CustomNameKeyProbe, closestdist));
						else if (obj.BroadcastRadius > closestdist)
							antenna.SetCustomName(string.Format("{0} {1}", CustomNameKeyProbe, direction.ToString()));
						else
							antenna.SetCustomName(string.Format("{0} {1} Broadcast Range: {2:N}", CustomNameKeyProbe, "No asteroids in range. ", obj.BroadcastRadius));
					}
					else
					{
						antenna.SetCustomName(string.Format("{0} {1}", CustomNameKeyProbe, "No asteroids detected."));
					}
				}
			}
			catch (Exception)
			{
				var antenna = (Sandbox.ModAPI.Ingame.IMyTerminalBlock)Entity;
				antenna.SetCustomName(string.Format("{0} Error.", CustomNameKeyProbe));
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
