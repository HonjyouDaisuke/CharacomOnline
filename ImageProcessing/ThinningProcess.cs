using SkiaSharp;
using CharacomOnline.Service;

namespace CharacomOnline.ImageProcessing;

public static class ThinningProcess {
  public static async Task<SKBitmap> ThinBinaryImageAsync(SKBitmap inputBitmap)
    {
        Console.WriteLine("Start--->");
        return await Task.Run(() => ThinBinaryImage(inputBitmap));
        Console.WriteLine("<-----End");
    }

  		
		
    public static SKBitmap? ThinBinaryImage(SKBitmap srcBitmap)
		{
            byte[,] src = ImageEffectService.ImageArrayFromBitmap(srcBitmap);
            int[,] b = new int[srcBitmap.Height + 2, srcBitmap.Width + 2];
            int[,] be = new int[srcBitmap.Height + 2, srcBitmap.Width + 2];
            int check, m;
            SKBitmap workBitmap = new SKBitmap(srcBitmap.Width, srcBitmap.Height);
            workBitmap = ImageEffectService.WhiteFilledBitmap(workBitmap);

            for (int i = 0; i < srcBitmap.Height; i++) {
                for (int j = 0; j < srcBitmap.Width; j++) {
                    b[i + 1, j + 1] = src[i, j];
                    be[i + 1, j + 1] = src[i, j];
                }
            }

            do
			{
				check = 0;
				for (int i = 0; i < srcBitmap.Height + 1; i++)
				{
					for (int j = 0; j < srcBitmap.Width + 1; j++)
					{
						//条件1：B(i,j)=1
						if (b[i+1, j+1] == 1) S_search(b, j+1, i+1, be);			
					}
				}

				//手順2
				m = 0;
				for(int i = 0; i < srcBitmap.Height + 1; i++)
				{
					for(int j=0;j<srcBitmap.Width+1;j++)
					{
						if(b[i+1,j+1]==-1)
						{
							b[i+1,j+1]=0;
							m++;
							check=1;
							be[i+1,j+1]=2;//変更した画素を保存しておく
						}
						else be[i+1,j+1]=b[i+1,j+1];
					}
				}
			}while(check!=0);


            for(int i=0;i<srcBitmap.Height;i++)
			{
				for(int j=0;j<srcBitmap.Width;j++)
				{
					if(b[i+1, j+1] != 1) continue;

						workBitmap.SetPixel(j, i, SKColors.Black);
						if(j+1 < workBitmap.Width) workBitmap.SetPixel(j+1, i, SKColors.Black);
						if(i+1 < workBitmap.Height)workBitmap.SetPixel(j, i+1, SKColors.Black);
						if(j+1 < workBitmap.Width && i+1 < workBitmap.Height)workBitmap.SetPixel(j+1, i+1, SKColors.Black);
					
				}
			}
			//BitmapStretchCopy(workBmp, outBmp);
			
			//workBmp.Dispose();
            return workBitmap;
            
		}
        public static void S_search(int[,] b,int x,int y,int[,] be)
		{
			byte [] c = new byte[9];
			byte [] cp = new byte[9];
			byte [] xi = new byte[9];
			
			// 8連結データ配列
			int [] py = new int[9] {0,-1,-1,-1,0,1,1,1,0};
			int [] px = new int[9] {1,1,0,-1,-1,-1,0,1,1};

			int i,n2,n1,s1,s2,s3,k,m,bx;
			
			n2=n1=s1=s2=s3=m=0;
	
	
			// 条件2：境界画素である条件
			for(i=0;i<8;i=i+2)
			{
				s1+=1-Math.Abs(b[y+py[i],x+px[i]]);
			}
	
			// 条件3：端点を削除しない条件
			for(i=0;i<8;i++)
			{
				s2+=Math.Abs(b[y+py[i],x+px[i]]);
			}
	
			// 条件4：孤立点を保存する条件
			// Ckの決定
			for(i=0;i<8;i++)
			{	
				if(b[y+py[i],x+px[i]] == 1) c[i]=1;
				else c[i]=0;
			}	
			for(i=0;i<8;i++)
			{
				s3+=c[i];
			}
	
			// 条件5：連結性を保存する条件
			for(i=0;i<9;i++)
			{
				if(Math.Abs(b[y+py[i],x+px[i]]) == 1) cp[i]=1;
				else cp[i]=0;
			}
			// Nc8(p0)を求める
			for(i=0;i<8;i=i+2)
				n1+=((1-Math.Abs(cp[i]))-(1-Math.Abs(cp[i]))*(1-Math.Abs(cp[i+1]))*(1-Math.Abs(cp[i+2])));

			// 条件6：線幅2に対する片側削除条件

			// 格納
			for(i=0;i<9;i++)
			{
				xi[i]=(byte)Math.Abs(b[y+py[i],x+px[i]]);
			}
			m=0;
			for(i=0;i<8;i++)
			{
				n2=0;
				bx=xi[i];
				xi[i]=0;// 0とする
				// Nc8(p0)を求める
				for(k=0;k<8;k=k+2)
					n2+=((1-Math.Abs(xi[k]))-(1-Math.Abs(xi[k]))*(1-Math.Abs(xi[k+1]))*(1-Math.Abs(xi[k+2])));
				// もとにもどす
				xi[i]=(byte)bx;

				if(n2==1 || b[y+py[i],x+px[i]]!=-1)
				{
					m++;
				}
			}
	
			//B(i,j)を-1にする(条件1から条件6をすべて満たすもの）
			if(b[y,x]==1 && s1>=1 && s2>=2 && s3>=1 && n1==1 && m==8)
				b[y,x]=-1;
		}
		/**
        #region 細線化サーチ処理
		public void S_search(int[,] b,int x,int y,int[,] be)
		{
			byte [] c = new byte[9];
			byte [] cp = new byte[9];
			byte [] xi = new byte[9];
			
			// 8連結データ配列
			int [] py = new int[9] {0,-1,-1,-1,0,1,1,1,0};
			int [] px = new int[9] {1,1,0,-1,-1,-1,0,1,1};

			int i,n2,n1,s1,s2,s3,k,m,bx;
			
			n2=n1=s1=s2=s3=m=0;
	
	
			// 条件2：境界画素である条件
			for(i=0;i<8;i=i+2)
			{
				s1+=1-Math.Abs(b[y+py[i],x+px[i]]);
			}
	
			// 条件3：端点を削除しない条件
			for(i=0;i<8;i++)
			{
				s2+=Math.Abs(b[y+py[i],x+px[i]]);
			}
	
			// 条件4：孤立点を保存する条件
			// Ckの決定
			for(i=0;i<8;i++)
			{	
				if(b[y+py[i],x+px[i]] == 1) c[i]=1;
				else c[i]=0;
			}	
			for(i=0;i<8;i++)
			{
				s3+=c[i];
			}
	
			// 条件5：連結性を保存する条件
			for(i=0;i<9;i++)
			{
				if(Math.Abs(b[y+py[i],x+px[i]]) == 1) cp[i]=1;
				else cp[i]=0;
			}
			// Nc8(p0)を求める
			for(i=0;i<8;i=i+2)
				n1+=((1-Math.Abs(cp[i]))-(1-Math.Abs(cp[i]))*(1-Math.Abs(cp[i+1]))*(1-Math.Abs(cp[i+2])));

			// 条件6：線幅2に対する片側削除条件

			// 格納
			for(i=0;i<9;i++)
			{
				xi[i]=(byte)Math.Abs(b[y+py[i],x+px[i]]);
			}
			m=0;
			for(i=0;i<8;i++)
			{
				n2=0;
				bx=xi[i];
				xi[i]=0;// 0とする
				// Nc8(p0)を求める
				for(k=0;k<8;k=k+2)
					n2+=((1-Math.Abs(xi[k]))-(1-Math.Abs(xi[k]))*(1-Math.Abs(xi[k+1]))*(1-Math.Abs(xi[k+2])));
				// もとにもどす
				xi[i]=(byte)bx;

				if(n2==1 || b[y+py[i],x+px[i]]!=-1)
				{
					m++;
				}
			}
	
			//B(i,j)を-1にする(条件1から条件6をすべて満たすもの）
			if(b[y,x]==1 && s1>=1 && s2>=2 && s3>=1 && n1==1 && m==8)
				b[y,x]=-1;
		}
		#endregion
		**/
		
}