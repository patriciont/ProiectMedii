# Space Finder: Project Documentation

## Project Name: Space Finder
**Platform**: .NET MAUI  
**Project Type**: Booking Application  
**Target Audience**: Institutions like universities, workplaces, community centers, bars, and any organization requiring event-based room bookings.

## Table of Contents
1. [Introduction](#1-introduction)
2. [Features](#2-features)
3. [System Overview](#3-system-overview)
    - [Models](#models)
    - [Views](#views)
4. [Functional Overview](#4-functional-overview)
    - [Admin Functionalities](#admin-functionalities)
    - [User Functionalities](#user-functionalities)
5. [System Architecture](#5-system-architecture)
    - [Folder Structure](#folder-structure)
    - [Key Code Files](#key-code-files)
6. [Database Design](#6-database-design)
7. [Future Improvements](#7-future-improvements)
8. [Conclusion](#8-conclusion)

---

## 1. Introduction
Space Finder is a dynamic booking application designed to manage room bookings for various institutions. It allows admins to create events, manage room availability, and define specific time slots for bookings. Users can view available rooms, book time slots, and manage their reservations through an intuitive calendar view.

The application offers a comprehensive experience for both admins and regular users, enabling full control over room bookings, user management, and event creation. Built with .NET MAUI, the app is cross-platform, delivering consistent experiences across mobile and desktop devices.

---

## 2. Features
- **Admin Control**: Multiple dashboards for managing rooms, users, and bookings.
- **User-Friendly Booking System**: View available rooms and timeslots and book accordingly.
- **Calendar View**: Users can track their bookings and easily cancel or reschedule them.
- **Dynamic Room Management**: Admins can open rooms for booking, define time frames, and set capacity for each timeslot.
- **User Profile Management**: Users can update their email addresses and passwords.
- **Signup and Authentication**: Users can sign up and log in securely with field validation to ensure data integrity.
- **Privacy Statement Compliance**: All users must agree to the privacy policy before accessing the app.

---

## 3. System Overview

### Models
- **AvailableDay**: Represents the days when a room is available for booking.
- **Booking**: Holds information about each booking made by a user.
- **BookingHistory**: Archive for past bookings that are no longer active.
- **BookingManager**: Manages booking operations, like creation, deletion, and history management.
- **CalendarEvent**: Represents events visible on the user's booking calendar.
- **Room**: Represents the room entity, including name, capacity, and type.
- **RoomSlot**: Time slots available for booking within a room.
- **User**: Represents the user entity, including their credentials and permissions.

### Views

#### Admin Views
- **AdminOverviewPage**: Displays all current bookings for the admin, with filtering options (by room, date, user, past/present bookings).
- **ManageUsers**: Admin dashboard for user management. Admins can create users with specific permissions, view user details, and delete users.
- **ManageBookingsPage**: Admin dashboard for managing all bookings with different buttons and functionalities based on permission levels.
- **MainPage**: The room dashboard for the admin to create, reset, delete rooms, and manage event days and time slots.

#### User Views
- **BookingDetails**: Displays details of a selected room, allowing users to book specific timeslots. Lists available dates and the number of booked slots per time frame.
- **BookingPage**: Lists rooms available for booking, filtered by the user’s interests or field of study.
- **EditProfilePage**: Users can edit their profile, including email and password.
- **ViewBookings**: Calendar-based view where users can see their bookings. Clicking on a date provides booking details for that day.

#### Shared Views
- **LoginPage**: Allows users to log in or sign up. All fields are validated to ensure data integrity.
- **OpeningPage**: The landing page, with buttons for booking rooms, viewing bookings, and accessing dashboards.
- **PrivacyStatementPage**: Displays the privacy statement that users must agree to during signup.
- **SignupPage**: Where new users can register. Field validation ensures proper input, and users must agree to the privacy policy.

---

## 4. Functional Overview

### Admin Functionalities
- **Room Management**: Admins can create rooms, set availability (days and timeslots), reset or delete rooms as necessary.
- **Booking Management**: Admins have a comprehensive dashboard to view all bookings and filter them by room, date, user, and past/present.
- **User Management**: Admins can create, modify, and delete users, setting permissions for each user based on their role.

### User Functionalities
- **Room Booking**: Users can browse rooms relevant to their interests or field of study and book time slots.
- **Calendar View**: Users can see a visual representation of their bookings and view details or cancel reservations on any given day.
- **Profile Management**: Users can update their email and password at any time.
- **Signup and Login**: New users can sign up, agree to the privacy statement, and securely log in.

---

## 5. System Architecture

### Folder Structure
- **Models**: Contains the entities used in the app such as AvailableDay, Booking, Room, etc.
- **Views**: The UI components such as AdminOverviewPage, BookingPage, EditProfilePage, etc.
- **ViewModels**: (If applicable) Would handle data-binding and business logic separate from the views.
- **Services**: Handles interactions with the backend (if present) or database management.

### Key Code Files
- **Models**: Represents the core entities of the app.
- **Views**: The actual screens/pages of the app, both for users and admins.
- **MainPage.xaml.cs**: The admin room management dashboard, allowing full control over room creation and timeslot allocation.

---

## 6. Database Design
- **Users Table**: Stores user data, including credentials and permission levels.
- **Rooms Table**: Stores information about rooms, including name, capacity, and availability.
- **Bookings Table**: Holds all active bookings made by users.
- **BookingHistory Table**: Holds archived bookings, moved from the active bookings table after the booking has passed.

---

## 7. Future Improvements
- **Enhanced Reporting**: Adding analytical tools to generate reports for admin users, such as booking trends or most popular rooms.
- **Notification System**: Users could receive notifications about upcoming bookings or changes.
- **Mobile App Optimization**: Further performance improvements for cross-platform usage.
- **Multi-Language Support**: Adding additional languages for global use.
- **Advanced Search & Filter**: Implementing more detailed filters for users and admins, such as booking status or user-specific filters.

---

## 8. Conclusion
Space Finder streamlines the room booking process for institutions, providing both admin and user-friendly interfaces with essential features like room management, bookings, and user control.
