﻿// <auto-generated />
using System;
using BrainBoxAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BrainBoxAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

            modelBuilder.Entity("BrainBoxAPI.Models.QuizModel", b =>
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

                    b.ToTable("QuizModels");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.QuizQuestionRelationModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AnswerOptionModelID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("QuestionModelID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("QuizModelID")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AnswerOptionModelID");

                    b.HasIndex("QuestionModelID");

                    b.HasIndex("QuizModelID");

                    b.ToTable("QuizQuestionRelationModels");
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

            modelBuilder.Entity("BrainBoxAPI.Models.QuizModel", b =>
                {
                    b.HasOne("BrainBoxAPI.Models.RoomModel", "Room")
                        .WithMany("Quizs")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.QuizQuestionRelationModel", b =>
                {
                    b.HasOne("BrainBoxAPI.Models.AnswerOptionModel", "SelectedAnswerOption")
                        .WithMany()
                        .HasForeignKey("AnswerOptionModelID");

                    b.HasOne("BrainBoxAPI.Models.QuestionModel", "Question")
                        .WithMany("QuizRelations")
                        .HasForeignKey("QuestionModelID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BrainBoxAPI.Models.QuizModel", "Quiz")
                        .WithMany("QuestionRelations")
                        .HasForeignKey("QuizModelID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");

                    b.Navigation("Quiz");

                    b.Navigation("SelectedAnswerOption");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.QuestionModel", b =>
                {
                    b.Navigation("AnswerOptions");

                    b.Navigation("QuizRelations");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.QuizModel", b =>
                {
                    b.Navigation("QuestionRelations");
                });

            modelBuilder.Entity("BrainBoxAPI.Models.RoomModel", b =>
                {
                    b.Navigation("Questions");

                    b.Navigation("Quizs");
                });
#pragma warning restore 612, 618
        }
    }
}
