using System;

public static class CoordinatesConverter
{
    public static double R = 128 / Math.PI;

    public static double ConvertDEGToRAD(double coordinate)
    {
        return coordinate * 0.017453292519943295; //rad = (reg / 180) * PI
    }

    private static double ConvertRADLonToDemAltTileX(double lonInRAD)
    {
        return R * (lonInRAD + Math.PI);
    }

    private static double ConvertToDemAltTileY(double latInRAD)
    {
        return (-1) * R / 2 * Math.Log((1 + Math.Sin(latInRAD)) / (1 - Math.Sin(latInRAD))) + 128;
    }

    private static double ConvertToPixelCoordinate(double demAltTileCoordinate, int zoomLevel)
    {
        return demAltTileCoordinate * Math.Pow(2, zoomLevel);
    }

    public static int LonToX(double degLon, int zoomLevel)
    {
        double radLon = ConvertDEGToRAD(degLon);
        double demAltTileX = ConvertRADLonToDemAltTileX(radLon);
        double px = ConvertToPixelCoordinate(demAltTileX, zoomLevel);
        return (int)Math.Floor(px / 256);
    }

    public static int LatToY(double degLat, int zoomLevel)
    {
        double radLat = ConvertDEGToRAD(degLat);
        double demAltTileY = ConvertToDemAltTileY(radLat);
        double py = ConvertToPixelCoordinate(demAltTileY, zoomLevel);
        return (int)Math.Floor(py / 256);
    }
}
