using System.Collections.Generic;
using System.Numerics;
using UnityTools.Algorithm.CalculateEigenvalues;
using UnityTools.SimpleQRAlgorithm;
using Vector3 = UnityEngine.Vector3;

namespace UnityTools.Physics.Obb
{
    public class OBB
    {
        public Vector3[] m_rot = new Vector3[3];	// rotation matrix for the transformation, stored as rows
        public Vector3	m_pos;		// translation component of the transformation
        public Vector3	m_ext;		// bounding box extents

        public Vector3 GetDir(int index)
        {
	        return new Vector3( m_rot[0][index], m_rot[1][index], m_rot[2][index] );
        }
        
        public Vector3 right
        {
	        get
	        {
		        return new Vector3( m_rot[0][0], m_rot[1][0], m_rot[2][0] );
	        }
        }
        public Vector3 up
        {
	        get
	        {
		        return new Vector3( m_rot[0][1], m_rot[1][1], m_rot[2][1] );
	        }
        }
        public Vector3 forward
        {
	        get
	        {
		        return new Vector3( m_rot[0][2], m_rot[1][2], m_rot[2][2] );
	        }
        }

	// method to set the OBB parameters which produce a box oriented according to
	// the covariance matrix C, which just containts the points pnts
	void build_from_covariance_matrix( float[,] C, List<Vector3> pnts ){
		// extract the eigenvalues and eigenvectors from C
		float[,] eigvec = new float[3, 3];
		float[] eigval = new float[3];
		QRAlgorithm.Diagonalize( C, out eigval, out eigvec );
		// Matrix3x3CalculateEigenvalues.DiagonalizeSymmetric3X3(C, out eigval, out eigvec);
		
		// find the right, up and forward vectors from the eigenvectors
		Vector3 r= new Vector3( eigvec[0,0], eigvec[1,0], eigvec[2,0] );
		Vector3 u= new Vector3( eigvec[0,1], eigvec[1,1], eigvec[2,1] );
		Vector3 f= new Vector3( eigvec[0,2], eigvec[1,2], eigvec[2,2] );
		r.Normalize();
		u.Normalize(); 
		f.Normalize();

		// set the rotation matrix using the eigvenvectors
		m_rot[0][0]=r.x; m_rot[0][1]=u.x; m_rot[0][2]=f.x;
		m_rot[1][0]=r.y; m_rot[1][1]=u.y; m_rot[1][2]=f.y;
		m_rot[2][0]=r.z; m_rot[2][1]=u.z; m_rot[2][2]=f.z;

		// now build the bounding box extents in the rotated frame
		Vector3 minim = new Vector3(1e10f, 1e10f, 1e10f);
		Vector3 maxim = new Vector3(-1e10f, -1e10f, -1e10f);
		for( int i=0; i<(int)pnts.Count; i++ ){
			Vector3 p_prime = new Vector3( Vector3.Dot(r,pnts[i]), 
				Vector3.Dot(u,pnts[i]), Vector3.Dot(f,pnts[i]) );
			minim = Vector3.Min(minim, p_prime );
			maxim = Vector3.Max(maxim, p_prime );
		}

		// set the center of the OBB to be the average of the 
		// minimum and maximum, and the extents be half of the
		// difference between the minimum and maximum
		Vector3 center = (maxim+minim)*0.5f;
		m_pos.Set( Vector3.Dot(m_rot[0],center), 
			Vector3.Dot(m_rot[1],center), 
			Vector3.Dot(m_rot[2],center) );
		m_ext = (maxim-minim)*0.5f;
	}

	public OBB(){ }

	// returns the volume of the OBB, which is a measure of
	// how tight the fit is.  Better OBBs will have smaller 
	// volumes
	double volume(){
		return 8*m_ext[0]*m_ext[1]*m_ext[2];
	}

	// constructs the corner of the aligned bounding box
	// in world space
	public void get_bounding_box( ref Vector3[] p ){
		Vector3 r= new Vector3( m_rot[0][0], m_rot[1][0], m_rot[2][0] );
		Vector3 u= new Vector3( m_rot[0][1], m_rot[1][1], m_rot[2][1] );
		Vector3 f= new Vector3( m_rot[0][2], m_rot[1][2], m_rot[2][2] );
		p[0] = m_pos - r*m_ext[0] - u*m_ext[1] - f*m_ext[2];
		p[1] = m_pos + r*m_ext[0] - u*m_ext[1] - f*m_ext[2];
		p[2] = m_pos + r*m_ext[0] - u*m_ext[1] + f*m_ext[2];
		p[3] = m_pos - r*m_ext[0] - u*m_ext[1] + f*m_ext[2];
		p[4] = m_pos - r*m_ext[0] + u*m_ext[1] - f*m_ext[2];
		p[5] = m_pos + r*m_ext[0] + u*m_ext[1] - f*m_ext[2];
		p[6] = m_pos + r*m_ext[0] + u*m_ext[1] + f*m_ext[2];
		p[7] = m_pos - r*m_ext[0] + u*m_ext[1] + f*m_ext[2];
	}

	

	// build an OBB from a vector of input points.  This
	// method just forms the covariance matrix and hands
	// it to the build_from_covariance_matrix() method
	// which handles fitting the box to the points
	public void build_from_points( List<Vector3> pnts ){
		Vector3 mu = new Vector3(0.0f, 0.0f, 0.0f);
		float[,] C = new float[3, 3];

		// loop over the points to find the mean point
		// location
		for( int i=0; i<(int)pnts.Count; i++ ){
			mu += pnts[i]/(pnts.Count);
		}

		// loop over the points again to build the 
		// covariance matrix.  Note that we only have
		// to build terms for the upper trianglular 
		// portion since the matrix is symmetric
		float cxx=0.0f, cxy=0.0f, cxz=0.0f, cyy=0.0f, cyz=0.0f, czz=0.0f;
		for( int i=0; i<(int)pnts.Count; i++ ){
			Vector3 p = pnts[i];
			cxx += p.x*p.x - mu.x*mu.x; 
			cxy += p.x*p.y - mu.x*mu.y; 
			cxz += p.x*p.z - mu.x*mu.z;
			cyy += p.y*p.y - mu.y*mu.y;
			cyz += p.y*p.z - mu.y*mu.z;
			czz += p.z*p.z - mu.z*mu.z;
		}

		// now build the covariance matrix
		C[0,0] = cxx; C[0,1] = cxy; C[0,2] = cxz;
		C[1,0] = cxy; C[1,1] = cyy; C[1,2] = cyz;
		C[2,0] = cxz; C[2,1] = cyz; C[2,2] = czz;

		// set the OBB parameters from the covariance matrix
		build_from_covariance_matrix( C, pnts );
	}
	//
	// // builds an OBB from triangles specified as an array of
	// // points with integer indices into the point array. Forms
	// // the covariance matrix for the triangles, then uses the
	// // method build_from_covariance_matrix() method to fit 
	// // the box.  ALL points will be fit in the box, regardless
	// // of whether they are indexed by a triangle or not.
	// void build_from_triangles( std::vector<Vec3> &pnts, std::vector<int> &tris ){
	// 	double Ai, Am=0.0;
	// 	Vec3 mu(0.0f, 0.0f, 0.0f), mui;
	// 	gmm::dense_matrix<double> C(3,3);
	// 	double cxx=0.0, cxy=0.0, cxz=0.0, cyy=0.0, cyz=0.0, czz=0.0;
	//
	// 	// loop over the triangles this time to find the
	// 	// mean location
	// 	for( int i=0; i<(int)tris.size(); i+=3 ){
	// 		Vec3 &p = pnts[tris[i+0]];
	// 		Vec3 &q = pnts[tris[i+1]];
	// 		Vec3 &r = pnts[tris[i+2]];
	// 		mui = (p+q+r)/3.0;
	// 		Ai = (q-p).Cross(r-p).Normalize()/2.0;
	// 		mu += mui*Ai;
	// 		Am += Ai;
	//
	// 		// these bits set the c terms to Am*E[xx], Am*E[xy], Am*E[xz]....
	// 		cxx += ( 9.0*mui.x*mui.x + p.x*p.x + q.x*q.x + r.x*r.x )*(Ai/12.0);
	// 		cxy += ( 9.0*mui.x*mui.y + p.x*p.y + q.x*q.y + r.x*r.y )*(Ai/12.0);
	// 		cxz += ( 9.0*mui.x*mui.z + p.x*p.z + q.x*q.z + r.x*r.z )*(Ai/12.0);
	// 		cyy += ( 9.0*mui.y*mui.y + p.y*p.y + q.y*q.y + r.y*r.y )*(Ai/12.0);
	// 		cyz += ( 9.0*mui.y*mui.z + p.y*p.z + q.y*q.z + r.y*r.z )*(Ai/12.0);
	// 	}
	// 	// divide out the Am fraction from the average position and 
	// 	// covariance terms
	// 	mu /= Am;
	// 	cxx /= Am; cxy /= Am; cxz /= Am; cyy /= Am; cyz /= Am; czz /= Am;
	//
	// 	// now subtract off the E[x]*E[x], E[x]*E[y], ... terms
	// 	cxx -= mu.x*mu.x; cxy -= mu.x*mu.y; cxz -= mu.x*mu.z;
	// 	cyy -= mu.y*mu.y; cyz -= mu.y*mu.z; czz -= mu.z*mu.z;
	//
	// 	// now build the covariance matrix
	// 	C(0,0)=cxx; C(0,1)=cxy; C(0,2)=cxz;
	// 	C(1,0)=cxy; C(1,1)=cyy; C(1,2)=cyz;
	// 	C(2,0)=cxz; C(1,2)=cyz; C(2,2)=czz;
	//
	// 	// set the obb parameters from the covariance matrix
	// 	build_from_covariance_matrix( C, pnts );
	// }
	//
	// // this code is for example purposes only and is likely to be inefficient.
	// // it simply builds the convex hull using CGAL, then tesselates the output
	// // of the convex hull and passes it to the build_from_triangles() method
	// // above.  
	// void build_from_convex_hull( std::vector<Vec3> &pnts ){
	// 	// build the convex hull. CGAL is a bit of a pain to use, but
	// 	// is certainly easier that writing your own convex hull code.
	// 	typedef CGAL::Exact_predicates_inexact_constructions_kernel  K;
	// 	typedef CGAL::Convex_hull_traits_3<K>						 Traits;
	// 	typedef Traits::Polyhedron_3								 Polyhedron_3;
	// 	typedef K::Point_3											 Point_3;
	//
	// 	// create a hull obect and a vector of points for CGAL to use
	// 	CGAL::Object			hull;
	// 	std::vector<Point_3>	cgal_points;
	// 	for( int i=0; i<(int)pnts.size(); i++ ){
	// 		cgal_points.push_back( Point_3( pnts[i].x, pnts[i].y, pnts[i].z ) );
	// 	}
	//
	// 	// call CGAL to build the convex hull
	// 	CGAL::convex_hull_3( cgal_points.begin(), cgal_points.end(), hull );
	//
	// 	// then build the OBB using only the triangles
	// 	// from the convex hull
	// 	Polyhedron_3 poly;
	// 	CGAL::assign(poly, hull);
	//
	// 	// if the convex hull is a polyhedron (which it should be), loop 
	// 	// over the facets of the convex hull and tesselate them. 
	// 	std::vector<Vec3> tpoints;
	// 	std::vector<int>  tindices;
	// 	for( Polyhedron_3::Facet_iterator iter=poly.facets_begin(); iter!=poly.facets_end(); iter++ ){
	// 		Point_3 p0 = (*iter).halfedge()->vertex()->point();
	// 		Point_3 p1 = (*iter).halfedge()->next()->vertex()->point();
	// 		Point_3 p2 = (*iter).halfedge()->next()->next()->vertex()->point();
	// 		tpoints.push_back( Vec3( p0.x(), p0.y(), p0.z() ) );
	// 		tpoints.push_back( Vec3( p1.x(), p1.y(), p1.z() ) );
	// 		tpoints.push_back( Vec3( p2.x(), p2.y(), p2.z() ) );
	// 		tindices.push_back( tindices.size() );
	// 		tindices.push_back( tindices.size() );
	// 		tindices.push_back( tindices.size() );
	// 	}
	//
	// 	// build the OBB using the triangles from the convex hull
	// 	build_from_triangles( tpoints, tindices );
	// }

    }
}