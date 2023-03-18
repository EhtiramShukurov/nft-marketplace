using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Xhibiter.Models;

namespace Xhibiter.DAL
{
    public class AppDbContext:IdentityDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<NFTCollection> NFTCollections { get; set; }
        public DbSet<NFT> NFTs { get; set; }
        public DbSet<NFTCategory> NFTCategories { get; set; }
        public DbSet<NFTMetadata> NFTMetadatas { get; set; }
        public DbSet<NFTUser> NFTUsers { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Activity> Activities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<NFT>()
    .HasOne(nft => nft.Collection)
    .WithMany(collection => collection.NFTs)
    .HasForeignKey(nft => nft.CollectionId)
    .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<NFTMetadata>()
            .HasOne(n => n.NFT)
            .WithOne(m => m.Metadata)
            .HasForeignKey<NFT>(m => m.MetadataId)
            .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Auction>()
        .HasOne(auction => auction.Winner)
        .WithMany(user => user.WonAuctions)
        .HasForeignKey(auction => auction.WinnerId)
        .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Sale>()
.HasOne(s => s.NFT)
.WithMany(n => n.Sales)
.HasForeignKey(s => s.NFTId)
.OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Bid>()
        .HasOne(b => b.Bidder)
        .WithMany(u => u.Bids)
        .HasForeignKey(b => b.BidderId)
        .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
