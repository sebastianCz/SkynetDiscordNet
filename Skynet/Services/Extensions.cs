using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Skynet.db;
using Skynet.Domain;
using Skynet.Domain.GuildData; 

namespace Skynet.Services
{
    public static class Extensions
    {
        public static List<LavalinkTrackBot> ConvertToBotTrack (this List<LavalinkTrack> enumerable)
        {
            var y = new List<LavalinkTrackBot>();
            enumerable.ForEach(x => { y.Add(x.ConvertToBotTrack());});
            return y;
        }
        
        public static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.RandomElementUsing<T>(new Random());
        }

        public static T RandomElementUsing<T>(this IEnumerable<T> enumerable, Random rand)
        {
            int index = rand.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }
        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = (float)new Random().NextDouble() * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value;

            }

            return default(T);
        }

        public static T GetRandomWeightedElement<T>(this IEnumerable<(T Item, int Weight)> weightedList)
        {
            if (weightedList == null || !weightedList.Any())
                throw new ArgumentException("Weighted list cannot be null or empty.");

            int totalWeight = weightedList.Sum(item => item.Weight);
            int randomNumber = new Random().Next(totalWeight);

            foreach (var (item, weight) in weightedList)
            {
                if (randomNumber < weight)
                {
                    
                    return item;
                }

                randomNumber -= weight;
            }

            // This should never happen if the weightedList is not empty and the sum of weights is greater than zero.
            throw new InvalidOperationException("Failed to get a random element from the weighted list.");
        } 
        public static LavalinkGuildConnection GetGuildConnection(this InteractionContext ctx)
        {

            var x = ctx.GetNode();
                var y = x.ConnectedGuilds.FirstOrDefault(x => x.Key == ctx.Member.Guild.Id).Value;
            return y;
        } 
        public static LavalinkNodeConnection GetNode(this InteractionContext ctx)
        {
           return ctx.Client.GetLavalink().ConnectedNodes.FirstOrDefault().Value;
        }
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

    }
 }

         
