using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ThornParser.Models.ParseModels
{
	public class MovingPlatformActor : BackgroundActor
	{
		protected string Image { get; }
		protected int Width { get; }
		protected int Height { get; }

		protected List<(double x, double y, double z, double angle, double opacity, int time)> Positions { get; } =
			new List<(double x, double y, double z, double angle, double opacity, int time)>();

		public MovingPlatformActor(string image, int width, int height, (int start, int end) lifespan) : base(lifespan)
		{
			Image = image;
			Width = width;
			Height = height;
		}

		protected class MovingPlatformSerializable : BackgroundSerializable
		{
			public string Image { get; set; }
			public int Height { get; set; }
			public int Width { get; set; }

			[JsonConverter(typeof(PositionConverter))]
			public (double x, double y, double z, double angle, double opacity, int time)[] Positions { get; set; }
		}

		public class PositionConverter : JsonConverter
		{
			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				var positions = ((double x, double y, double z, double angle, double opacity, int time)[]) value;
				writer.WriteStartArray();
				foreach (var position in positions)
				{
					(double x, double y, double z, double angle, double opacity, int time) = position;
					writer.WriteStartArray();
					writer.WriteValue(x);
					writer.WriteValue(y);
					writer.WriteValue(z);
					writer.WriteValue(angle);
					writer.WriteValue(opacity);
					writer.WriteValue(time);
					writer.WriteEndArray();
				}

				writer.WriteEndArray();
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
				JsonSerializer serializer)
			{
				throw new NotSupportedException();
			}

			public override bool CanRead => false;

			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof((double x, double y, double z, double angle, double opacity, int time));
			}
		}

		public void AddPosition(double x, double y, double z, double angle, double opacity, int time)
		{
			Positions.Add((x, y, z, angle, opacity, time));
		}

		public override GenericActorSerializable GetCombatReplayJSON(CombatReplayMap map)
		{
			var positions = Positions.OrderBy(x => x.time).Select(pos =>
			{
				(double mapX, double mapY) = map.GetMapCoord((float) pos.x, (float) pos.y);
				pos.x = mapX;
				pos.y = mapY;

				return pos;
			}).ToArray();

			MovingPlatformSerializable aux = new MovingPlatformSerializable
			{
				Type = "MovingPlatform",
				Image = Image,
				Width = Width,
				Height = Height,
				Start = Lifespan.Item1,
				End = Lifespan.Item2,
				Positions = positions
			};
			return aux;
		}
	}
}