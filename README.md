# OSRS Flipping tool

A filter-based Discord bot/Desktop client for detecting anomalies in OSRS item price data.
The live market data is fetched from the [OSRS-Wiki API](https://oldschool.runescape.wiki/w/RuneScape:Real-time_Prices).

When the Discord client detects an anomaly, it currently generates two dynamic graphs based on the available data.
The graphs have three lines:
- Green, for the "low" (instant sell) price,
- Orange, for the "high" (instant buy) price,
- Red, for the average trend of the prices.

## Status
[![.NET build](https://github.com/japsuu/osrs-flipper/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/japsuu/osrs-flipper/actions/workflows/dotnet.yml)

## Screenshots

![Dump detection 1](https://github.com/japsuu/osrs-flipper/assets/55388432/6b5a4234-bc45-4bfe-94a6-8f006d07f6ea)
![Dump detection 2](https://github.com/japsuu/osrs-flipper/assets/55388432/5f9e1804-6ce8-4d39-a205-89881b26da4a)

## Usage

Download the [latest release](https://github.com/japsuu/osrs-flipper/releases/latest) from the [Releases tab](https://github.com/japsuu/osrs-flipper/releases).

There are two executables available:
- The Discord client, used to spin up your own instance of the bot,
- The desktop client, used to run the program locally on your machine. This has no Discord integration.
