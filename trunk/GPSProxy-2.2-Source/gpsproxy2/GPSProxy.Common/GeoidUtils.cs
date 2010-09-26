/*
$Id: GeoidUtils.cs,v 1.1 2006/05/23 09:27:06 andrew_klopper Exp $

Copyright 2005-2006 Andrew Rowland Klopper (http://gpsproxy.sourceforge.net/)

This file is part of GPSProxy.

GPSProxy is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

GPSProxy is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with GPSProxy; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;

namespace GPSProxy.Common
{
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public class GeoidUtils
	{
		public const double a = 6378137;
		public const double b = 6356752.3142;
		public const double f = (a - b) / a;

		// <summary>
		// Calculate geodesic distance (in m) and bearing (in radians, clockwise from North) from the a specified
		// origin point to a specified destination. Elevation is not taken into account. All latitudes and
		// longitudes must be specified in radians.
		//
		// From: Vincenty inverse formula - T Vincenty, "Direct and Inverse Solutions of Geodesics on the 
		//       Ellipsoid with application of nested equations", Survey Review, vol XXII no 176, 1975
		//       http://www.ngs.noaa.gov/PUBS_LIB/inverse.pdf
		// </summary>
		static public void GetDistanceAndBearing(double originLatRadians, double originLonRadians,
			double destLatRadians, double destLonRadians, out double distance, out double bearing)
		{
			const double accuracy = 1e-12;
			const int maxIterations = 20;

			double L = destLonRadians - originLonRadians;
			double U1 = Math.Atan((1 - f) * Math.Tan(originLatRadians));
			double U2 = Math.Atan((1 - f) * Math.Tan(destLatRadians));
			double sinU1 = Math.Sin(U1);
			double cosU1 = Math.Cos(U1);
			double sinU2 = Math.Sin(U2);
			double cosU2 = Math.Cos(U2);
			double lambda = L;
			double lambdaP = 2 * Math.PI;

			// We assume that the loop is executed at least once. The
			// values assigned below are purely to satisfy the compiler.
			double sinLambda = 0, cosLambda = 0;
			double cosSqAlpha = 0, sigma = 0, sinSigma = 0, cosSigma = 0, cos2SigmaM = 0;
			int remainingIterations = maxIterations;
			while ((Math.Abs(lambda - lambdaP) > accuracy) && (--remainingIterations > 0)) 
			{
				sinLambda = Math.Sin(lambda);
				cosLambda = Math.Cos(lambda);
				sinSigma = Math.Sqrt((cosU2 * sinLambda) * (cosU2 * sinLambda) +
					(cosU1 * sinU2 - sinU1 * cosU2 * cosLambda) *
					(cosU1 * sinU2 - sinU1 * cosU2 * cosLambda));
				cosSigma = sinU1 * sinU2 + cosU1 * cosU2 * cosLambda;
				sigma = Math.Atan2(sinSigma, cosSigma);
				double alpha = sinSigma == 0 ? 0 // Empirically, alpha -> 0 as sigma -> 0
					: Math.Asin(cosU1 * cosU2 * sinLambda / sinSigma);
				cosSqAlpha = Math.Cos(alpha);
				cosSqAlpha *= cosSqAlpha;
				cos2SigmaM = cosSigma - 2 * sinU1 * sinU2 / cosSqAlpha;
				double C = f / 16 * cosSqAlpha * (4 + f * (4 - 3 * cosSqAlpha));
				lambdaP = lambda;
				lambda = L + (1 - C) * f * Math.Sin(alpha) *
					(sigma + C * sinSigma *
					(cos2SigmaM + C * cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM)));
			}

			// Test for the case where lon2 - lon1 is very close to 2 * PI
			// (points close together on either side of the longitude 180 line) in which
			// case the loop might not be entered.
			if (remainingIterations == maxIterations)
				throw new Exception("Distance calculation loop not entered.");

			// Check if we actually got an answer.
			if (remainingIterations == 0)
				throw new Exception("Distance calculation failed to converge.");

			double uSq = cosSqAlpha * (a * a - b * b) / (b * b);
			double A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
			double B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
			double deltaSigma = B * sinSigma * (cos2SigmaM + B / 4 * (cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM) -
				B / 6 * cos2SigmaM * (-3 + 4 * sinSigma * sinSigma) * (-3 + 4 * cos2SigmaM * cos2SigmaM)));

			distance = b * A * (sigma - deltaSigma);
			bearing = Math.Atan2(cosU2 * sinLambda,
				cosU1 * sinU2 - sinU1 * cosU2 * cosLambda);
		}

		static public void GetECEFCoordinates(double latRadians, double lonRadians, double altAboveWGS84, out double x, out double y, out double z)
		{
			double h = altAboveWGS84;
			double aC = GeoidUtils.a / Math.Sqrt(Math.Pow(Math.Cos(latRadians), 2) + Math.Pow((1 - GeoidUtils.f) * Math.Sin(latRadians), 2));
			double aS = Math.Pow(1 - GeoidUtils.f, 2) * aC;
			x = (aC + h) * Math.Cos(latRadians) * Math.Cos(lonRadians);
			y = (aC + h) * Math.Cos(latRadians) * Math.Sin(lonRadians);
			z = (aS + h) * Math.Sin(latRadians);
		}
	}
}
