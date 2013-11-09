// Copyright (c) Igor Grešovnik (2008 - present), IGLib license; http://www2.arnes.si/~ljc3m2/igor/iglib/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Kitware.VTK;


using IG.Lib;
using IG.Num;

namespace IG.Gr3d
{


    /// <summary>Various VTK utilities that extend functionality of the ActiViz VTK wrappers.</summary>
    /// $A Igor Sep11;
    public static class UtilVtk
    {


        #region Auxiliary

        
        /// <summary>Returns true if the specified renderer contains the specified actor.</summary>
        /// <param name="renderer">Vtk renderer that is queried for containing the specified actor.</param>
        /// <param name="actor">Actor for which we querry whether it is contained.</param>
        public static bool ContainsActor(vtkRenderer renderer, vtkActor actor)
        {
            if (renderer == null || actor == null)
                return false;
            vtkActorCollection actors = renderer.GetActors();
            if (actors != null)
            {
                vtkActor currentActor;
                actors.InitTraversal();
                do
                {
                    currentActor = actors.GetNextActor();
                    if (currentActor != null)
                    {
                        if (currentActor == actor)
                            return true;
                    }
                } while (currentActor != null);
            }
            return false;
        }


        /// <summary>Returns the first non-null VTK actor contained in the specified <see cref="vtkRenderer"/> object.</summary>
        /// <param name="renderer">VTK renderer window (type) <see cref="vtkRenderWindow"/> for which a contained renderer is returned.</param>
        public static vtkActor GetFirstActor(vtkRenderer renderer)
        {
            if (renderer != null)
            {
                vtkActorCollection actors = renderer.GetActors();
                vtkActor currentActor;
                actors.InitTraversal();
                do
                {
                    currentActor = actors.GetNextItem();
                    if (currentActor != null)
                        return currentActor;
                } while (currentActor != null);
            }
            return null;
        }


        /// <summary>Returns number of actors contained on the specified <see cref="vtkRenderer"/> object.
        /// If the specified renderer is null then 0 returned.</summary>
        /// <param name="win">VTK renderer (type <see cref="vtkRenderer"/>) for which the number of contained
        /// actors is returned.</param>
        public static int GetNumActors(vtkRenderer renderer)
        {
            if (renderer != null)
            {
                vtkActorCollection actors = renderer.GetActors();
                if (actors != null)
                    return actors.GetNumberOfItems();
            }
            return 0;
        }


        /// <summary>Returns a list of all non-null renderers contained in the specified <see cref="vtkRenderWindow"/> object.
        /// If VTK render window is null then an empty list is returned.</summary>
        /// <param name="win">VTK renderer window (type) <see cref="vtkRenderWindow"/> for which a list of contained
        /// renderers is returned.</param>
        public static List<vtkActor> GetActors(vtkRenderer renderer)
        {
            List<vtkActor> ret = new List<vtkActor>();
            if (renderer != null)
            {
                vtkActorCollection actors = renderer.GetActors();
                vtkActor currentActor;
                actors.InitTraversal();
                do
                {
                    currentActor = actors.GetNextItem();
                    if (currentActor != null)
                        ret.Add(currentActor);
                } while (currentActor != null);
            }
            return ret;
        }

        /// <summary>Returns an array of all non-null actors contained in the specified <see cref="vtkRenderer"/> object.
        /// If the specified VTK renderer is null then an empty array is returned.</summary>
        /// <param name="renderer">VTK renderer (type <see cref="vtkRenderer"/>) for which an array of contained
        /// renderers is returned.</param>
        public static vtkActor[] GetActorsArray(vtkRenderer renderer)
        {
            List<vtkActor> ret = GetActors(renderer);
            if (ret == null)
                return new vtkActor[0];
            else
                return ret.ToArray();
        }

        /// <summary>Removes all actors form the specified renderer.</summary>
        /// <param name="renderer">The renderer from which all actors are removed.</param>
        public static void RemoveAllActors(vtkRenderer renderer)
        {
            if (renderer != null)
            {
                vtkActorCollection actors = renderer.GetActors();
                if (actors != null)
                    actors.RemoveAllItems();
            }
        }


        /// <summary>Returns true if the specified renderer contains the specified actor2D.</summary>
        /// <param name="renderer">Vtk renderer that is queried for containing the specified actor2D.</param>
        /// <param name="actor">Actor2D for which we querry whether it is contained.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public static bool ContainsActor2D(vtkRenderer renderer, vtkActor2D actor2D)
        {
            if (renderer == null || actor2D == null)
                return false;
            vtkActor2DCollection actors2D = renderer.GetActors2D();

            if (actors2D != null)
            {
                vtkActor2D currentActor2D;
                actors2D.InitTraversal();
                do
                {
                    currentActor2D = actors2D.GetNextActor2D();
                    if (currentActor2D != null)
                    {
                        if (currentActor2D == actor2D)
                            return true;
                    }
                } while (currentActor2D != null);
            }
            return false;
        }


        /// <summary>Returns the first non-null VTK actor2D contained in the specified <see cref="vtkRenderer"/> object.</summary>
        /// <param name="renderer">VTK renderer window (type) <see cref="vtkRenderWindow"/> for which a contained renderer is returned.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public static vtkActor2D GetFirstActor2D(vtkRenderer renderer)
        {
            if (renderer != null)
            {
                vtkActor2DCollection actors2D = renderer.GetActors2D();
                vtkActor2D currentActor2D;
                actors2D.InitTraversal();
                do
                {
                    currentActor2D = actors2D.GetNextItem();
                    if (currentActor2D != null)
                        return currentActor2D;
                } while (currentActor2D != null);
            }
            return null;
        }


        /// <summary>Returns number of actors2D contained on the specified <see cref="vtkRenderer"/> object.
        /// If the specified renderer is null then 0 returned.</summary>
        /// <param name="win">VTK renderer (type <see cref="vtkRenderer"/>) for which the number of contained
        /// actors2D is returned.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public static int GetNumActors2D(vtkRenderer renderer)
        {
            if (renderer != null)
            {
                vtkActor2DCollection actors2D = renderer.GetActors2D();
                if (actors2D != null)
                    return actors2D.GetNumberOfItems();
            }
            return 0;
        }


        /// <summary>Returns a list of all non-null renderers contained in the specified <see cref="vtkRenderWindow"/> object.
        /// If VTK render window is null then an empty list is returned.</summary>
        /// <param name="win">VTK renderer window (type) <see cref="vtkRenderWindow"/> for which a list of contained
        /// renderers is returned.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public static List<vtkActor2D> GetActors2D(vtkRenderer renderer)
        {
            List<vtkActor2D> ret = new List<vtkActor2D>();
            if (renderer != null)
            {
                vtkActor2DCollection actors2D = renderer.GetActors2D();
                vtkActor2D currentActor2D;
                actors2D.InitTraversal();
                do
                {
                    currentActor2D = actors2D.GetNextItem();
                    if (currentActor2D != null)
                        ret.Add(currentActor2D);
                } while (currentActor2D != null);
            }
            return ret;
        }

        /// <summary>Returns an array of all non-null actors2D contained in the specified <see cref="vtkRenderer"/> object.
        /// If the specified VTK renderer is null then an empty array is returned.</summary>
        /// <param name="renderer">VTK renderer (type <see cref="vtkRenderer"/>) for which an array of contained
        /// renderers is returned.</param>
        /// $A Igor Oct11, Tako78 Dec13;
        public static vtkActor2D[] GetActors2DArray(vtkRenderer renderer)
        {
            List<vtkActor2D> ret = GetActors2D(renderer);
            if (ret == null)
                return new vtkActor2D[0];
            else
                return ret.ToArray();
        }

        /// <summary>Removes all actors form the specified renderer.</summary>
        /// <param name="renderer">The renderer from which all actors are removed.</param>
        public static void RemoveAllActors2D(vtkRenderer renderer)
        {
            if (renderer != null)
            {
                vtkActor2DCollection actors2D = renderer.GetActors2D();
                if (actors2D != null)
                    actors2D.RemoveAllItems();
            }
        }


        /// <summary>Returns true if the specified renderer window contains the specified renderer.</summary>
        /// <param name="renderer">Vtk renderer window that is queried for containing the specified renderer.</param>
        /// <param name="actor">Renderer for which we querry whether it is contained.</param>
        public static bool ContainsRenderer(vtkRenderWindow window, vtkRenderer renderer)
        {
            if (window==null || renderer==null)
                return false;
            else
                return (window.HasRenderer(renderer)!=0);
        }

        /// <summary>Returns the first non-null VTK renderrer contained in the specified <see cref="vtkRenderWindow"/> object.</summary>
        /// <param name="win">VTK renderer window (type <see cref="vtkRenderWindow"/>) for which a contained renderer is returned.</param>
        /// <returns></returns>
        public static vtkRenderer GetFirstRenderer(
            vtkRenderWindow win)
        {
            if (win != null)
            {
                vtkRendererCollection renderers = win.GetRenderers();
                vtkRenderer currentRenderer;
                renderers.InitTraversal();
                do
                {
                    currentRenderer = renderers.GetNextItem();
                    if (currentRenderer != null)
                        return currentRenderer;
                } while (currentRenderer != null);
            }
            return null;
        }


        /// <summary>Returns number of renderers contained on the specified <see cref="vtkRenderWindow"/> object.
        /// If VTK render window is null then 0 returned.</summary>
        /// <param name="win">VTK renderer window (type <see cref="vtkRenderWindow"/>) for which the number of contained
        /// renderers is returned.</param>
        public static int GetNumRenderers(vtkRenderWindow win)
        {
            if (win != null)
            {
                vtkRendererCollection renderers = win.GetRenderers();
                if (renderers!=null) 
                    return renderers.GetNumberOfItems();
            }
            return 0;
        }


        /// <summary>Returns a list of all non-null renderers contained in the specified <see cref="vtkRenderWindow"/> object.
        /// If VTK render window is null then an empty list is returned.</summary>
        /// <param name="win">VTK renderer window (type <see cref="vtkRenderWindow"/>) for which a list of contained
        /// renderers is returned.</param>
        public static List<vtkRenderer> GetRenderers(vtkRenderWindow win)
        {
            List<vtkRenderer> ret = new List<vtkRenderer>();
            if (win != null)
            {
                vtkRendererCollection renderers = win.GetRenderers();
                vtkRenderer currentRenderer;
                renderers.InitTraversal();
                do
                {
                    currentRenderer = renderers.GetNextItem();
                    if (currentRenderer!=null)
                        ret.Add(currentRenderer);
                } while (currentRenderer!=null);
            }
            return ret;
        }

        /// <summary>Returns an array of all non-null renderers contained in the specified <see cref="vtkRenderWindow"/> object.
        /// If the specified VTK render window is null then an empty array is returned.</summary>
        /// <param name="win">VTK renderer window (type <see cref="vtkRenderWindow"/>) for which an array of contained
        /// renderers is returned.</param>
        public static vtkRenderer[] GetRenderersArray(vtkRenderWindow win)
        {
            List<vtkRenderer> ret = GetRenderers(win);
            if (ret == null)
                return new vtkRenderer[0];
            else
                return ret.ToArray();
        }





        #endregion Auxiliary

        
        #region Bounds


        public static void SetBounds(vtkCubeAxesActor actor, IBoundingBox bounds)
        {
            if (actor == null)
                throw new ArgumentException("VTK Actor to set bounds on is not specified (null reference).");
            if (bounds != null)
            {
                int dim = actor.GetBounds().Length / 2;
                if (bounds.Dimension != dim)
                    throw new ArgumentException("Dimension of bounding box is different than dimension of cube ais actor.");
                else if (dim != 3)
                    throw new InvalidOperationException("Setting bounds on axis actor is not implemented for dimensions different than 3.");
                {
                    actor.SetBounds(
                        bounds.GetMin(0),
                        bounds.GetMax(0),
                        bounds.GetMin(1),
                        bounds.GetMax(1),
                        bounds.GetMin(2),
                        bounds.GetMax(2)
                        );
                }
            }
        }


        /// <summary>Creates and returns the minimal volume bounding box that contains all graphical objects
        /// of the specified <see cref="vtkRenderer"/>.</summary>
        /// <param name="actors"><see cref="vtkRenderer"/>objects for which the bounding box is created.
        /// Must not be null or empty array.</param>
        public static BoundingBox CreateBounds(params vtkRenderer[] renderers)
        {
            BoundingBox ret = null;
            if (renderers == null)
                throw new ArgumentException("VTK renderers for which bounding box should be created are not specified (null reference).");
            else if (renderers.Length < 1)
                throw new ArgumentException("VTK renderers for which bounding box should be created are not specified (empty array).");
            else
            {
                for (int i = 0; i < renderers.Length; ++i)
                {
                    vtkRenderer currentRenderer = renderers[i];
                    if (currentRenderer != null)
                    {
                        vtkActorCollection actors = currentRenderer.GetActors();
                        if (actors != null)
                        {
                            vtkActor currentActor;
                            actors.InitTraversal();
                            do
                            {
                                currentActor = actors.GetNextActor();
                                if (currentActor != null)
                                {
                                    if (ret == null)
                                    {
                                        ret = CreateBounds(currentActor);
                                    } else
                                        UpdateBounds(ret, currentActor);
                                }
                            } while (currentActor != null);
                        }
                    }
                }
                if (ret == null)
                    throw new ArgumentException("Could not create bounding box on basis of provided renderers.");
            }
            return ret;
        }

        /// <summary>Updates the specified bounding box in such a way that it also contains all graphic primitives of the
        /// specified VTK Renderers.</summary>
        /// <param name="renderers">VTK Renderers for which the bounding box is eventually resized in such a way
        /// that the renderers fit in it.
        /// All non-null elements must have the same dimension.</param>
        /// <param name="bounds">Bounding box to be updated. It must be allocated and of the same dimension as the VTK 
        /// actor according to which the bounding box is updated.</param>
        public static void UpdateBounds(IBoundingBox bounds, params vtkRenderer[] renderers)
        {
            if (bounds==null && renderers!=null)
                throw new ArgumentNullException("Bounding box to be updated by bounds of VTK Renderers is not specified (null reference).");
            if (renderers != null)
            {
                for (int i = 0; i < renderers.Length; ++i)
                {
                    vtkRenderer currentRenderer = renderers[i];
                    if (currentRenderer != null)
                    {
                        vtkActorCollection actors = currentRenderer.GetActors();
                        if (actors != null)
                        {
                            vtkActor currentActor;
                            actors.InitTraversal();
                            do
                            {
                                currentActor = actors.GetNextActor();
                                if (currentActor != null)
                                {
                                    UpdateBounds(bounds, currentActor);
                                }
                            } while (currentActor != null);
                        }
                    }
                }
            }
        }


        /// <summary>Updates the specified bounding box in such a way that it also contains all graphic primitives of the
        /// specified VTK Renderers.</summary>
        /// <param name="renderers">VTK Renderers for which the bounding box is eventually resized in such a way
        /// that the renderers fit in it.
        /// All non-null elements must have the same dimension.</param>
        /// <param name="bounds">Bounding box to be updated. It must be allocated and of the same dimension as the VTK 
        /// actor according to which the bounding box is updated.</param>
        public static void UpdateBounds(ref IBoundingBox bounds, params vtkRenderer[] renderers)
        {
            if (renderers != null)
            {
                int dim = 0;
                for (int i = 0; i < renderers.Length; ++i)
                {
                    vtkRenderer currentRenderer = renderers[i];
                    if (currentRenderer != null)
                    {
                        vtkActorCollection actors = currentRenderer.GetActors();
                        if (actors != null)
                        {
                            vtkActor currentActor;
                            actors.InitTraversal();
                            do
                            {
                                currentActor = actors.GetNextActor();
                                if (currentActor != null)
                                {
                                    if (dim == 0)
                                    {
                                        dim = currentActor.GetBounds().Length / 2;
                                        if (bounds == null)
                                            bounds = new BoundingBox(dim);
                                        else if (bounds.Dimension != dim)
                                            bounds = new BoundingBox(dim);
                                    }
                                    else
                                    {
                                        if (2*dim!=currentActor.GetBounds().Length)
                                            throw new InvalidOperationException("Actor on renderer No. " + i + " is of incompatible dimension ("
                                            + (currentActor.GetBounds().Length / 2) + " instead of " + dim + ").");

                                    }
                                    UpdateBounds(bounds, currentActor);
                                }
                            } while (currentActor != null);
                        }
                    }
                }
            }
        }

        /// <summary>Creates and returns the minimal volume bounding box that contains all graphical objects
        /// of the specified <see cref="vtkActor"/>.</summary>
        /// <param name="actors"><see cref="vtkActor"/>objects for which the bounding box is created.
        /// Must not be null or empty array.</param>
        public static BoundingBox CreateBounds(params vtkActor[] actors)
        {
            BoundingBox ret = null;
            if (actors==null)
                throw new ArgumentException("VTK actors for which bounding box should be created are not specified (null reference).");
            else if (actors.Length<1)
                throw new ArgumentException("VTK actors for which bounding box should be created are not specified (empty array).");
            else
            {
                int dim = 0;
                for (int i = 0; i < actors.Length; ++i)
                {
                    vtkActor currentActor = actors[i];
                    if (currentActor != null)
                    {
                        if (ret == null)
                        {
                            dim = currentActor.GetBounds().Length / 2;
                            if (dim > 0)
                                ret = new BoundingBox(dim);
                        }
                        if (ret!=null)
                            UpdateBounds(ret, currentActor);
                    }
                }
                if (ret == null)
                    throw new ArgumentException("Could not create bounding box on basis of provided actors.");
            }
            return ret;
        }

        /// <summary>Updates the specified bounding box in such a way that it also contains all graphic primitives of the
        /// specified VTK Actors.</summary>
        /// <param name="actors">VTK Actors for which the bounding box is eventually resized in such a way
        /// that the actor fits in it. If different than null then the first element must not be null.
        /// All non-null elements must have the same dimension.</param>
        /// <param name="bounds">Bounding box to be updated. It must be allocated and of the same dimension as the VTK 
        /// actor according to which the bounding box is updated.</param>
        public static void UpdateBounds(IBoundingBox bounds, params vtkActor[] actors)
        {
            if (bounds==null && actors!=null)
                throw new ArgumentNullException("Bounding box to be updated by bounds of VTK Actors is not specified (null reference).");
            if (actors != null)
            {
                double[] boundsArray = actors[0].GetBounds();
                int dim = boundsArray.Length / 2;
                if (bounds.Dimension != dim)
                    throw new ArgumentException("Bounding box to be updated with VTK Actor objects is of wrong dimension ("
                        + dim + " instead of " + bounds.Dimension + ").");
                for (int i=0; i<actors.Length; ++i)
                {
                    vtkActor currentActor = actors[i];
                    if (currentActor != null)
                    {
                        boundsArray = currentActor.GetBounds();
                        if (boundsArray.Length != 2 * dim)
                            throw new InvalidOperationException("Can not set bounds for a group of actors. Actor No. " + i + " is of incompatible dimension.");
                        for (int j = 0; j < dim; ++j)
                        {
                            bounds.Update(j, boundsArray[2 * j]);
                            bounds.Update(j, boundsArray[2 * j + 1]);
                        }
                    }
                }
           }
        }
         
        /// <summary>Updates the specified bounding box in such a way that it also contains all graphic primitives of the
        /// specified VTK Actor.</summary>
        /// <param name="actors">VTK Actor for which the bounding box is eventually resized in such a way
        /// that the actor fits in it. If null then noting happens.</param>
        /// <param name="bounds">Bounding box to be updated. If allocated then it must be of the same dimension as 
        /// <paramref name="actors"/>.</param>
        /// <exception cref="ArgumentException">When bounding box is not of the same dimension than <paramref name="actors"/>
        /// and <paramref name="actors"/> is different than 0.</exception>
        public static void UpdateBounds(ref IBoundingBox bounds, params vtkActor[] actors)
        {
            if (actors != null)
            {
                int dim = 0;
                for (int i = 0; i < actors.Length; ++i)
                {
                    vtkActor actor = actors[i];
                    if (actor != null)
                    {
                        if (dim == 0)
                        {
                            dim = actor.GetBounds().Length / 2;
                            if (bounds == null)
                                bounds = new BoundingBox(dim);
                            else if (bounds.Dimension != dim)
                                bounds = new BoundingBox(dim);
                        } else
                        {
                            if (2 * dim != actor.GetBounds().Length)
                                throw new InvalidOperationException("Actor No. " + i + " is of incompatible dimension ("
                            + (actor.GetBounds().Length / 2) + " instead of " + dim + ").");
                        }
                        UpdateBounds(bounds, actor);
                    }
                }
           }
        }

        #endregion Bounds


        #region IVtkFormContainer.ExtensionMethods

        /// <summary>Returns a boolean value telling whether the specified object of type <see cref="IVtkFormContainer"/>
        /// actually contains a VTK rendering control (type <see cref="RenderWindowControl"/>).</summary>
        /// <param name="control">Control (or more generally control container) that is checked.</param>
        /// <remarks><para>There is a possibility that its appropriate sub-control is of another type such as Panel, and
        /// the object just emulates a container of VTK render control. This may be the case because of conditional
        /// compilation, which enables manipulation of forms by form designer, since the VTK form is not compatible with
        /// the designer.</para></remarks>
        public static bool IsVtkForm(this IVtkFormContainer control)
        {
            bool ret = false;
            Kitware.VTK.RenderWindowControl vtkControl = control.VtkRenderWindowControl as Kitware.VTK.RenderWindowControl;
            if (vtkControl != null)
                ret = true;
            return ret;
        }

        /// <summary>Returns a VTK rendering control of type <see cref="Kitware.VTK.RenderWindowControl"/> that is
        /// contained in the specified object, or null if such a control is not contained or is null.</param>
        /// <param name="control">Control (or more generally control container) whose rendering control is attempted
        /// to be returned.</param>
        /// <remarks><para>There is a possibility that its appropriate sub-control is of another type such as Panel, and
        /// the object just emulates a container of VTK render control. This may be the case because of conditional
        /// compilation, which enables manipulation of forms by form designer, since the VTK form is not compatible with
        /// the designer.</para></remarks>
        public static Kitware.VTK.RenderWindowControl GetVtkRenderWindowControl(this IVtkFormContainer control)
        {
            Kitware.VTK.RenderWindowControl vtkControl = control.VtkRenderWindowControl as Kitware.VTK.RenderWindowControl;
            return vtkControl;
        }

        /// <summary>Returns a VTK rendering window of type <see cref="Kitware.VTK.vtkRenderWindow"/> that is
        /// contained in the specified object, or null if such a control is not contained or is null.</param>
        /// <param name="control">Control (or more generally control container) whose rendering window is attempted
        /// to be returned.</param>
        /// <remarks><para>There is a possibility that its appropriate sub-control is of another type such as Panel, and
        /// the object just emulates a container of VTK render control. This may be the case because of conditional
        /// compilation, which enables manipulation of forms by form designer, since the VTK form is not compatible with
        /// the designer.</para></remarks>
        public static Kitware.VTK.vtkRenderWindow GetVtkRenderWindow(this IVtkFormContainer control)
        {
            Kitware.VTK.vtkRenderWindow vtkRenderWindow = null;
            Kitware.VTK.RenderWindowControl vtkControl = control.VtkRenderWindowControl as Kitware.VTK.RenderWindowControl;
            if (vtkControl != null)
                vtkRenderWindow = vtkControl.RenderWindow;
            return vtkRenderWindow;
        }

        /// <summary>Returns a VTK renderer of type <see cref="Kitware.VTK.vtkRenderer"/> that is
        /// contained in the specified object, or null if such a VTK render control is not contained or its renderer is null.</param>
        /// <param name="control">Control (or more generally control container) whose VTK renderer is attempted
        /// to be returned.</param>
        /// <remarks><para>There is a possibility that its appropriate sub-control is of another type such as Panel, and
        /// the object just emulates a container of VTK render control. This may be the case because of conditional
        /// compilation, which enables manipulation of forms by form designer, since the VTK form is not compatible with
        /// the designer.</para></remarks>
        public static Kitware.VTK.vtkRenderer GetVtkRenderer(this IVtkFormContainer control)
        {
            Kitware.VTK.vtkRenderer renderer = null;
            Kitware.VTK.vtkRenderWindow vtkRenderWindow = null;
            Kitware.VTK.RenderWindowControl vtkControl = control.VtkRenderWindowControl as Kitware.VTK.RenderWindowControl;
            if (vtkControl != null)
                vtkRenderWindow = vtkControl.RenderWindow;
            if (vtkRenderWindow!=null)
                renderer = vtkRenderWindow.GetRenderers().GetFirstRenderer();
            return renderer;
        }

        /// <summary>Returns the active VTK camera of type <see cref="Kitware.VTK.vtkCamera"/> that is
        /// contained in the specified object, or null if VTK render control is not contained or its camera is null.</param>
        /// <param name="control">Control (or more generally control container) whose VTK camera is attempted
        /// to be returned.</param>
        /// <remarks><para>There is a possibility that its appropriate sub-control is of another type such as Panel, and
        /// the object just emulates a container of VTK render control. This may be the case because of conditional
        /// compilation, which enables manipulation of forms by form designer, since the VTK form is not compatible with
        /// the designer.</para></remarks>
        public static Kitware.VTK.vtkCamera GetVtkCamera(this IVtkFormContainer control)
        {
            Kitware.VTK.vtkCamera camera = null;
            Kitware.VTK.vtkRenderer renderer = null;
            Kitware.VTK.vtkRenderWindow vtkRenderWindow = null;
            Kitware.VTK.RenderWindowControl vtkControl = control.VtkRenderWindowControl as Kitware.VTK.RenderWindowControl;
            if (vtkControl != null)
            {
                vtkRenderWindow = vtkControl.RenderWindow;
                if (vtkRenderWindow != null)
                {
                    renderer = vtkRenderWindow.GetRenderers().GetFirstRenderer();
                    if (renderer != null)
                        camera = renderer.GetActiveCamera();
                }
            }
            return camera;
        }

        #endregion IVtkFormContainer.ExtensionMethods

        /// <summary>
        /// Transform colorScale range to LookUpTable range directly.
        /// </summary>
        /// <param name="scalarBarColorScale">Color scale.</param>
        /// <param name="numValues">Number of cells in ScalarBar.</param>
        /// <param name="ScalarBarLookUpTable">LookUpTable</param>
        /// Tako78 Dec23;
        public static void LookUpTableRange(ColorScale scalarBarColorScale, int numValues, ref vtkLookupTable ScalarBarLookUpTable)
        {
            if (scalarBarColorScale == null)
                throw new ArgumentNullException("No color scale defined.");
            if (numValues == 0)
                numValues = 1;
            if (ScalarBarLookUpTable == null)
                ScalarBarLookUpTable = vtkLookupTable.New();
            
            double minCS = scalarBarColorScale.MinValue;
            double maxCS = scalarBarColorScale.MaxValue;

            LookUpTableRange(scalarBarColorScale, numValues, minCS, maxCS, ref ScalarBarLookUpTable);
        }


        /// <summary>
        /// Transform colorScale range to LookUpTable range. Ranges are different. 
        /// </summary>
        /// <param name="scalarBarColorScale">Color scale.</param>
        /// <param name="numValues">Number of cells in ScalarBar.</param>
        /// <param name="minRangeLT">Minimum range in LookUpTable.</param>
        /// <param name="maxRangeLT">Maximum range in LookUpTable.</param>
        /// <param name="ScalarBarLookUpTable">LookUpTable</param>
        /// Tako78 Dec23;
        public static void LookUpTableRange(ColorScale scalarBarColorScale, int numValues, double minRangeLT, double maxRangeLT, 
            ref vtkLookupTable ScalarBarLookUpTable)
        {
            if (scalarBarColorScale == null)
                throw new ArgumentNullException("No color scale defined.");
            if (ScalarBarLookUpTable == null)
                ScalarBarLookUpTable = vtkLookupTable.New();
            if (minRangeLT >= maxRangeLT)
                throw new ArgumentException("Minimum value can not be bigger than maximim value.");

            double minCS = scalarBarColorScale.MinValue;
            double maxCS = scalarBarColorScale.MaxValue;

            ScalarBarLookUpTable.SetNumberOfTableValues(numValues);
            ScalarBarLookUpTable.SetNumberOfColors(numValues);
            ScalarBarLookUpTable.SetTableRange(minRangeLT, maxRangeLT);
            double halfstep = 0.5 * (maxRangeLT - minRangeLT) / (double)numValues;
            for (int i = 0; i < numValues; ++i)
            {
                double valueLT = halfstep + minRangeLT + (double)i * (maxRangeLT - minRangeLT) / (double)numValues;
                double valueCS = minCS + (valueLT - minRangeLT) * (maxCS - minCS) / (maxRangeLT - minRangeLT);
                //Console.WriteLine(i + ": value on LT: " + valueLT + ", CS: " + valueCS);
                //Console.WriteLine("Value on CS: " + valueCS);
                color col = scalarBarColorScale.GetColor(valueCS);
                ScalarBarLookUpTable.SetTableValue(i, col.R, col.G, col.B, col.Opacity);
            }
        }
    
    
    } // class UtilVtk


}
