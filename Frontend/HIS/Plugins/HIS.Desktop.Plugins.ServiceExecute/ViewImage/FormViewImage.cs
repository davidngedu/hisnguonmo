﻿using DevExpress.Utils;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute.ViewImage
{
    public partial class FormViewImage : Form
    {
        private ADO.ImageADO ImagePreview;
        private List<ADO.ImageADO> ListImage;
        string currentItem = "";

        public FormViewImage()
        {
            InitializeComponent();
        }

        public FormViewImage(List<ADO.ImageADO> listImage, ADO.ImageADO imagePreview)
            : this()
        {
            try
            {
                ImagePreview = imagePreview;
                currentItem = ImagePreview.FileName;
                ListImage = listImage;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormViewImage_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                ProcessListDataImage();

                ProcessImagePreview();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListDataImage()
        {
            try
            {
                if (ListImage != null && ListImage.Count > 0)
                {
                    int index = 0;
                    foreach (var image in ListImage)
                    {
                        image.ImageIndex = index++;
                        TileItem tileNew = new TileItem();
                        tileNew.Image = image.IMAGE_DISPLAY;
                        tileNew.ImageScaleMode = TileItemImageScaleMode.Stretch;
                        tileNew.Name = image.FileName;
                        tileNew.ItemClick += TileItemClick;
                        if (ImagePreview != null && image == ImagePreview)
                        {
                            tileNew.Checked = true;
                        }
                        tileGroup2.Items.Add(tileNew);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TileItemClick(object sender, TileItemEventArgs e)
        {
            try
            {
                foreach (var item in tileGroup2.Items)
                {
                    item.Checked = false;
                }
                PictureEdit.Image = e.Item.Image;
                PictureEdit.Properties.ZoomPercent = 100;
                PictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
                e.Item.Checked = true;
                currentItem = e.Item.Name;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessImagePreview()
        {
            try
            {
                if (ImagePreview != null)
                {
                    PictureEdit.Image = ImagePreview.IMAGE_DISPLAY;
                    PictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        int zoom = 40;

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            try
            {
                PictureEdit.Properties.ZoomPercent = PictureEdit.Properties.ZoomPercent + zoom;
                PictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
                PictureEdit.Properties.PictureAlignment = ContentAlignment.MiddleCenter;
                PictureEdit.Refresh();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            try
            {
                PictureEdit.Properties.ZoomPercent = PictureEdit.Properties.ZoomPercent - zoom;
                PictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
                PictureEdit.Properties.PictureAlignment = ContentAlignment.MiddleCenter;

                PictureEdit.Refresh();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var item in tileGroup2.Items)
                {
                    item.Checked = false;
                }
                this.currentItem = tileGroup2.Items[0].Name;
                PictureEdit.Image = tileGroup2.Items[0].Image;
                PictureEdit.Properties.ZoomPercent = 100;
                PictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
                tileGroup2.Items[0].Checked = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var item in tileGroup2.Items)
                {
                    item.Checked = false;
                }
                PictureEdit.Image = tileGroup2.Items[tileGroup2.Items.Count - 1].Image;
                this.currentItem = tileGroup2.Items[tileGroup2.Items.Count - 1].Name;
                PictureEdit.Properties.ZoomPercent = 100;
                PictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
                tileGroup2.Items[tileGroup2.Items.Count - 1].Checked = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                ADO.ImageADO previewItem = null;
                foreach (var item in ListImage)
                {
                    if (this.currentItem == item.FileName)
                    {
                        previewItem = ListImage[item.ImageIndex - 1];
                    }
                }
                this.currentItem = previewItem.FileName;

                foreach (var item in tileGroup2.Items)
                {
                    item.Checked = false;
                    if (previewItem.FileName == ((TileItem)item).Name)
                    {
                        item.Checked = true;
                    }
                }

                PictureEdit.Image = previewItem.IMAGE_DISPLAY;
                PictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                ADO.ImageADO previewItem = null;
                foreach (var item in ListImage)
                {
                    if (this.currentItem == item.FileName)
                    {
                        previewItem = ListImage[item.ImageIndex + 1];
                    }
                }
                this.currentItem = previewItem.FileName;

                foreach (var item in tileGroup2.Items)
                {
                    item.Checked = false;
                    if (previewItem.FileName == ((TileItem)item).Name)
                    {
                        item.Checked = true;
                    }
                }

                PictureEdit.Image = previewItem.IMAGE_DISPLAY;
                PictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Clip;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        float zoomSpeedFactor = 0.02f;
        private void PictureEdit_Properties_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                PictureEdit.Properties.ZoomPercent += e.Delta * zoomSpeedFactor;
                DXMouseEventArgs.GetMouseArgs(e).Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormViewImage
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourceFormViewImage = new ResourceManager("HIS.Desktop.Plugins.ServiceExecute.Resources.Lang", typeof(FormViewImage).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormViewImage.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourceFormViewImage, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("FormViewImage.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResourceFormViewImage, LanguageManager.GetCulture());
                this.tileControl.Text = Inventec.Common.Resource.Get.Value("FormViewImage.tileControl.Text", Resources.ResourceLanguageManager.LanguageResourceFormViewImage, LanguageManager.GetCulture());
                this.PictureEdit.Properties.NullText = Inventec.Common.Resource.Get.Value("FormViewImage.PictureEdit.Properties.NullText", Resources.ResourceLanguageManager.LanguageResourceFormViewImage, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormViewImage.Text", Resources.ResourceLanguageManager.LanguageResourceFormViewImage, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FormViewImage_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                currentItem = null;
                ListImage = null;
                ImagePreview = null;
                zoom = 0;
                this.btnPreview.Click -= new System.EventHandler(this.btnPreview_Click);
                this.btnNext.Click -= new System.EventHandler(this.btnNext_Click);
                this.btnZoomIn.Click -= new System.EventHandler(this.btnZoomIn_Click);
                this.btnZoomOut.Click -= new System.EventHandler(this.btnZoomOut_Click);
                this.btnLast.Click -= new System.EventHandler(this.btnLast_Click);
                this.btnFirst.Click -= new System.EventHandler(this.btnFirst_Click);
                this.PictureEdit.Properties.MouseWheel -= new System.Windows.Forms.MouseEventHandler(this.PictureEdit_Properties_MouseWheel);
                this.Load -= new System.EventHandler(this.FormViewImage_Load);
                emptySpaceItem2 = null;
                emptySpaceItem1 = null;
                layoutControlItem3 = null;
                layoutControlItem9 = null;
                layoutControlItem8 = null;
                layoutControlItem7 = null;
                layoutControlItem6 = null;
                layoutControlItem5 = null;
                layoutControlItem4 = null;
                Root = null;
                btnFirst = null;
                btnLast = null;
                btnZoomOut = null;
                btnZoomIn = null;
                btnNext = null;
                btnPreview = null;
                layoutControl2 = null;
                tileGroup2 = null;
                layoutControlItem2 = null;
                tileControl = null;
                layoutControlItem1 = null;
                PictureEdit = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
