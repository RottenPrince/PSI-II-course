﻿// <auto-generated />
using System;
using BrainBoxAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BrainBoxAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231107020304_UnusedCleanup")]
    partial class UnusedCleanup
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.13");

            modelBuilder.Entity("BrainBoxAPI.Models.AnswerOptionModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OptionText")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("QuestionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("AnswerOptions");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.QuestionModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ImageSource")
                        .HasColumnType("TEXT");

                    b.Property<int>("RoomId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.QuestionSolveRunJoinModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AnswerOptionModelID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("QuestionModelID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SolveRunModelID")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AnswerOptionModelID");

                    b.HasIndex("QuestionModelID");

                    b.HasIndex("SolveRunModelID");

                    b.ToTable("QuestionSolveRunJoinModels");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.RoomModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.SolveRunModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoomId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.ToTable("SolveRunModels");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.AnswerOptionModel", b =>
                {
                    b.HasOne("BrainBoxAPI.Models.QuestionModel", "Question")
                        .WithMany("AnswerOptions")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.QuestionModel", b =>
                {
                    b.HasOne("BrainBoxAPI.Models.RoomModel", "Room")
                        .WithMany("Questions")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.QuestionSolveRunJoinModel", b =>
                {
                    b.HasOne("BrainBoxAPI.Models.AnswerOptionModel", "SelectedAnswerOption")
                        .WithMany()
                        .HasForeignKey("AnswerOptionModelID");

                    b.HasOne("BrainBoxAPI.Models.QuestionModel", "Question")
                        .WithMany("Joins")
                        .HasForeignKey("QuestionModelID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BrainBoxAPI.Models.SolveRunModel", "SolveRun")
                        .WithMany("SolveRunJoin")
                        .HasForeignKey("SolveRunModelID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");

                    b.Navigation("SelectedAnswerOption");

                    b.Navigation("SolveRun");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.SolveRunModel", b =>
                {
                    b.HasOne("BrainBoxAPI.Models.RoomModel", "Room")
                        .WithMany("SolveRuns")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.QuestionModel", b =>
                {
                    b.Navigation("AnswerOptions");

                    b.Navigation("Joins");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.RoomModel", b =>
                {
                    b.Navigation("Questions");

                    b.Navigation("SolveRuns");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.SolveRunModel", b =>
                {
                    b.Navigation("SolveRunJoin");
                });
#pragma warning restore 612, 618
        }
    }
}
