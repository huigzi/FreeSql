using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FreeSql.Tests.MySql {
	
	public class MySqlSelectTest {

		ISelect<Topic> select => g.mysql.Select<Topic>();

		[Table(Name = "tb_topic")]
		public class Topic {
			[Column(IsIdentity = true, IsPrimary = true)]
			public int Id { get; set; }
			public int Clicks { get; set; }
			public int TestTypeInfoGuid { get; set; }
			public TestTypeInfo Type { get; set; }
			public string Title { get; set; }
			public DateTime CreateTime { get; set; }

			public virtual TopicFields Fields { get; set; }
		}
		public class TopicFields {
			[Column(IsPrimary = true)]
			public int TopicId { get; set; }
			public virtual Topic Topic { get; set; }
		}
		public class TestTypeInfo {
			[Column(IsIdentity = true)]
			public int Guid { get; set; }

			public int ParentId { get; set; }
			public virtual TestTypeParentInfo Parent { get; set; }

			public string Name { get; set; }

			public virtual ICollection<Topic> Topics { get; set; }
		}
		public class TestTypeParentInfo {
			[Column(IsIdentity = true)]
			public int Id { get; set; }
			public string Name { get; set; }

			public List<TestTypeInfo> Types { get; set; }
		}

		public partial class Song {
			[Column(IsIdentity = true)]
			public int Id { get; set; }
			public DateTime? Create_time { get; set; }
			public bool? Is_deleted { get; set; }
			public string Title { get; set; }
			public string Url { get; set; }

			public virtual ICollection<Tag> Tags { get; set; }
		}
		public partial class Song_tag {
			public int Song_id { get; set; }
			public virtual Song Song { get; set; }

			public int Tag_id { get; set; }
			public virtual Tag Tag { get; set; }
		}
		public partial class Tag {
			[Column(IsIdentity = true)]
			public int Id { get; set; }
			public int? Parent_id { get; set; }
			public virtual Tag Parent { get; set; }

			public decimal? Ddd { get; set; }
			public string Name { get; set; }

			public virtual ICollection<Song> Songs { get; set; }
			public virtual ICollection<Tag> Tags { get; set; }
		}


		[Fact]
		public void Lazy() {
			var tags = g.mysql.Select<Tag>().Where(a => a.Parent.Name == "xxx")
				.LeftJoin(a => a.Parent_id == a.Parent.Id)
				.ToSql();

			var songs = g.mysql.Select<Song>().Limit(10).ToList();


		}

		[Fact]
		public void ToList() {
			var t0 = select.Limit(50).ToList();


			var t1 = g.mysql.Select<TestInfo>().Where("").Where(a => a.Id > 0).Skip(100).Limit(200).ToSql();
			var t2 = g.mysql.Select<TestInfo>().As("b").Where("").Where(a => a.Id > 0).Skip(100).Limit(200).ToSql();


			var sql1 = select.LeftJoin(a => a.Type.Guid == a.TestTypeInfoGuid).ToSql();
			var sql2 = select.LeftJoin<TestTypeInfo>((a, b) => a.TestTypeInfoGuid == b.Guid && b.Name == "111").ToSql();
			var sql3 = select.LeftJoin("TestTypeInfo b on b.Guid = a.TypeGuid").ToSql();

			//g.mysql.Select<TestInfo, TestTypeInfo, TestTypeParentInfo>().Join((a, b, c) => new Model.JoinResult3(
			//   Model.JoinType.LeftJoin, a.TypeGuid == b.Guid,
			//   Model.JoinType.InnerJoin, c.Id == b.ParentId && c.Name == "xxx")
			//);

			//var sql4 = select.From<TestTypeInfo, TestTypeParentInfo>((a, b, c) => new SelectFrom()
			//	.InnerJoin(a.TypeGuid == b.Guid)
			//	.LeftJoin(c.Id == b.ParentId)
			//	.Where(b.Name == "xxx"))
			//.Where(a => a.Id == 1).ToSql();

			var sql4 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				.InnerJoin(a => a.TestTypeInfoGuid == b.Guid)
				.LeftJoin(a => c.Id == b.ParentId)
				.Where(a => b.Name == "xxx")).ToSql();
			//.Where(a => a.Id == 1).ToSql();


			var list111 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				.InnerJoin(a => a.TestTypeInfoGuid == b.Guid)
				.LeftJoin(a => c.Id == b.ParentId)
				.Where(a => b.Name != "xxx"));
			var list111sql = list111.ToSql();
			var list111data = list111.ToList((a, b, c) => new {
					a.Id,
					title_substring = a.Title.Substring(0, 1),
					a.Type,
					ccc = new { a.Id, a.Title },
					tp = a.Type,
					tp2 = new {
						a.Id,
						tp2 = a.Type.Name
					},
					tp3 = new {
						a.Id,
						tp33 = new {
							a.Id
						}
					}
				});

			var ttt122 = g.mysql.Select<TestTypeParentInfo>().Where(a => a.Id > 0).ToSql();
			var sql5 = g.mysql.Select<TestInfo>().From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s).Where((a, b, c) => a.Id == b.ParentId).ToSql();
			var t11112 = g.mysql.Select<TestInfo>().ToList(a => new {
				a.Id,
				a.Title,
				a.Type,
				ccc = new { a.Id, a.Title },
				tp = a.Type,
				tp2 = new {
					a.Id,
					tp2 = a.Type.Name
				},
				tp3 = new {
					a.Id,
					tp33 = new {
						a.Id
					}
				}

			});

			var t100 = g.mysql.Select<TestInfo>().Where("").Where(a => a.Id > 0).Skip(100).Limit(200).Caching(50).ToList();
			var t101 = g.mysql.Select<TestInfo>().As("b").Where("").Where(a => a.Id > 0).Skip(100).Limit(200).Caching(50).ToList();


			var t1111 = g.mysql.Select<TestInfo>().ToList(a => new { a.Id, a.Title, a.Type });

			var t2222 = g.mysql.Select<TestInfo>().ToList(a => new { a.Id, a.Title, a.Type.Name });
		}
		[Fact]
		public void ToOne() {
		}
		[Fact]
		public void ToSql() {
		}
		[Fact]
		public void Any() {
			var count = select.Where(a => 1 == 1).Count();
			Assert.False(select.Where(a => 1 == 2).Any());
			Assert.Equal(count > 0, select.Where(a => 1 == 1).Any());
		}
		[Fact]
		public void Count() {
			var count = select.Where(a => 1 == 1).Count();
			select.Where(a => 1 == 1).Count(out var count2);
			Assert.Equal(count, count2);
			Assert.Equal(0, select.Where(a => 1 == 2).Count());
		}
		[Fact]
		public void Master() {
			Assert.StartsWith(" SELECT", select.Master().Where(a => 1 == 1).ToSql());
		}
		[Fact]
		public void Caching() {
			var result1 = select.Where(a => 1 == 1).Caching(20, "testcaching").ToList();
			var testcaching1 = g.mysql.Cache.Get("testcaching");
			Assert.NotNull(testcaching1);
			var result2 = select.Where(a => 1 == 1).Caching(20, "testcaching").ToList();
			var testcaching2 = g.mysql.Cache.Get("testcaching");
			Assert.NotNull(testcaching2);
			Assert.Equal(result1.Count, result1.Count);
		}
		[Fact]
		public void From() {
			//�������
			var query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				 .LeftJoin(a => a.TestTypeInfoGuid == b.Guid)
				 .LeftJoin(a => b.ParentId == c.Id)
				);
			var sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` b ON a.`TestTypeInfoGuid` = b.`Guid` LEFT JOIN `TestTypeParentInfo` c ON b.`ParentId` = c.`Id`", sql);
			query2.ToList();
		}
		[Fact]
		public void LeftJoin() {
			//����е�������a.Type��a.Type.Parent ���ǵ�������
			var query = select.LeftJoin(a => a.Type.Guid == a.TestTypeInfoGuid);
			var sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid`", sql);
			query.ToList();

			query = select.LeftJoin(a => a.Type.Guid == a.TestTypeInfoGuid && a.Type.Name == "xxx");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid` AND a__Type.`Name` = 'xxx'", sql);
			query.ToList();

			query = select.LeftJoin(a => a.Type.Guid == a.TestTypeInfoGuid && a.Type.Name == "xxx").Where(a => a.Type.Parent.Id == 10);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeParentInfo` a__Type__Parent ON 1 = 1 LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid` AND a__Type.`Name` = 'xxx' WHERE (a__Type__Parent.`Id` = 10)", sql);
			query.ToList();

			//���û�е�������
			query = select.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` b ON b.`Guid` = a.`TestTypeInfoGuid`", sql);
			query.ToList();

			query = select.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid && b.Name == "xxx");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` b ON b.`Guid` = a.`TestTypeInfoGuid` AND b.`Name` = 'xxx'", sql);
			query.ToList();

			query = select.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid && b.Name == "xxx").Where(a => a.Type.Parent.Id == 10);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeParentInfo` b__Parent ON 1 = 1 LEFT JOIN `TestTypeInfo` b ON b.`Guid` = a.`TestTypeInfoGuid` AND b.`Name` = 'xxx' WHERE (b__Parent.`Id` = 10)", sql);
			query.ToList();

			//�������
			query = select
				.LeftJoin(a => a.Type.Guid == a.TestTypeInfoGuid)
				.LeftJoin(a => a.Type.Parent.Id == a.Type.ParentId);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid` LEFT JOIN `TestTypeParentInfo` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId`", sql);
			query.ToList();

			query = select
				.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid)
				.LeftJoin<TestTypeParentInfo>((a, c) => c.Id == a.Type.ParentId);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` b ON b.`Guid` = a.`TestTypeInfoGuid` LEFT JOIN `TestTypeParentInfo` c ON c.`Id` = b.`ParentId`", sql);
			query.ToList();

			//���û�е�������b��c������ϵ
			var query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				 .LeftJoin(a => a.TestTypeInfoGuid == b.Guid)
				 .LeftJoin(a => b.ParentId == c.Id));
			sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeInfo` b ON a.`TestTypeInfoGuid` = b.`Guid` LEFT JOIN `TestTypeParentInfo` c ON b.`ParentId` = c.`Id`", sql);
			query2.ToList();

			//������϶����㲻��
			query = select.LeftJoin("TestTypeInfo b on b.Guid = a.TestTypeInfoGuid");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN TestTypeInfo b on b.Guid = a.TestTypeInfoGuid", sql);
			query.ToList();

			query = select.LeftJoin("TestTypeInfo b on b.Guid = a.TestTypeInfoGuid and b.Name = ?bname", new { bname = "xxx" });
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN TestTypeInfo b on b.Guid = a.TestTypeInfoGuid and b.Name = ?bname", sql);
			query.ToList();
		}
		[Fact]
		public void InnerJoin() {
			//����е�������a.Type��a.Type.Parent ���ǵ�������
			var query = select.InnerJoin(a => a.Type.Guid == a.TestTypeInfoGuid);
			var sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid`", sql);
			query.ToList();

			query = select.InnerJoin(a => a.Type.Guid == a.TestTypeInfoGuid && a.Type.Name == "xxx");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid` AND a__Type.`Name` = 'xxx'", sql);
			query.ToList();

			query = select.InnerJoin(a => a.Type.Guid == a.TestTypeInfoGuid && a.Type.Name == "xxx").Where(a => a.Type.Parent.Id == 10);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeParentInfo` a__Type__Parent ON 1 = 1 INNER JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid` AND a__Type.`Name` = 'xxx' WHERE (a__Type__Parent.`Id` = 10)", sql);
			query.ToList();

			//���û�е�������
			query = select.InnerJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` b ON b.`Guid` = a.`TestTypeInfoGuid`", sql);
			query.ToList();

			query = select.InnerJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid && b.Name == "xxx");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` b ON b.`Guid` = a.`TestTypeInfoGuid` AND b.`Name` = 'xxx'", sql);
			query.ToList();

			query = select.InnerJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid && b.Name == "xxx").Where(a => a.Type.Parent.Id == 10);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeParentInfo` b__Parent ON 1 = 1 INNER JOIN `TestTypeInfo` b ON b.`Guid` = a.`TestTypeInfoGuid` AND b.`Name` = 'xxx' WHERE (b__Parent.`Id` = 10)", sql);
			query.ToList();

			//�������
			query = select
				.InnerJoin(a => a.Type.Guid == a.TestTypeInfoGuid)
				.InnerJoin(a => a.Type.Parent.Id == a.Type.ParentId);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid` INNER JOIN `TestTypeParentInfo` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId`", sql);
			query.ToList();

			query = select
				.InnerJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid)
				.InnerJoin<TestTypeParentInfo>((a, c) => c.Id == a.Type.ParentId);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` b ON b.`Guid` = a.`TestTypeInfoGuid` INNER JOIN `TestTypeParentInfo` c ON c.`Id` = b.`ParentId`", sql);
			query.ToList();

			//���û�е�������b��c������ϵ
			var query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				 .InnerJoin(a => a.TestTypeInfoGuid == b.Guid)
				 .InnerJoin(a => b.ParentId == c.Id));
			sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN `TestTypeInfo` b ON a.`TestTypeInfoGuid` = b.`Guid` INNER JOIN `TestTypeParentInfo` c ON b.`ParentId` = c.`Id`", sql);
			query2.ToList();

			//������϶����㲻��
			query = select.InnerJoin("TestTypeInfo b on b.Guid = a.TestTypeInfoGuid");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN TestTypeInfo b on b.Guid = a.TestTypeInfoGuid", sql);
			query.ToList();

			query = select.InnerJoin("TestTypeInfo b on b.Guid = a.TestTypeInfoGuid and b.Name = ?bname", new { bname = "xxx" });
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a INNER JOIN TestTypeInfo b on b.Guid = a.TestTypeInfoGuid and b.Name = ?bname", sql);
			query.ToList();

		}
		[Fact]
		public void RightJoin() {
			//����е�������a.Type��a.Type.Parent ���ǵ�������
			var query = select.RightJoin(a => a.Type.Guid == a.TestTypeInfoGuid);
			var sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid`", sql);
			query.ToList();

			query = select.RightJoin(a => a.Type.Guid == a.TestTypeInfoGuid && a.Type.Name == "xxx");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid` AND a__Type.`Name` = 'xxx'", sql);
			query.ToList();

			query = select.RightJoin(a => a.Type.Guid == a.TestTypeInfoGuid && a.Type.Name == "xxx").Where(a => a.Type.Parent.Id == 10);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeParentInfo` a__Type__Parent ON 1 = 1 RIGHT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid` AND a__Type.`Name` = 'xxx' WHERE (a__Type__Parent.`Id` = 10)", sql);
			query.ToList();

			//���û�е�������
			query = select.RightJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` b ON b.`Guid` = a.`TestTypeInfoGuid`", sql);
			query.ToList();

			query = select.RightJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid && b.Name == "xxx");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` b ON b.`Guid` = a.`TestTypeInfoGuid` AND b.`Name` = 'xxx'", sql);
			query.ToList();

			query = select.RightJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid && b.Name == "xxx").Where(a => a.Type.Parent.Id == 10);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a LEFT JOIN `TestTypeParentInfo` b__Parent ON 1 = 1 RIGHT JOIN `TestTypeInfo` b ON b.`Guid` = a.`TestTypeInfoGuid` AND b.`Name` = 'xxx' WHERE (b__Parent.`Id` = 10)", sql);
			query.ToList();

			//�������
			query = select
				.RightJoin(a => a.Type.Guid == a.TestTypeInfoGuid)
				.RightJoin(a => a.Type.Parent.Id == a.Type.ParentId);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid` RIGHT JOIN `TestTypeParentInfo` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId`", sql);
			query.ToList();

			query = select
				.RightJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid)
				.RightJoin<TestTypeParentInfo>((a, c) => c.Id == a.Type.ParentId);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` b ON b.`Guid` = a.`TestTypeInfoGuid` RIGHT JOIN `TestTypeParentInfo` c ON c.`Id` = b.`ParentId`", sql);
			query.ToList();

			//���û�е�������b��c������ϵ
			var query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				 .RightJoin(a => a.TestTypeInfoGuid == b.Guid)
				 .RightJoin(a => b.ParentId == c.Id));
			sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN `TestTypeInfo` b ON a.`TestTypeInfoGuid` = b.`Guid` RIGHT JOIN `TestTypeParentInfo` c ON b.`ParentId` = c.`Id`", sql);
			query2.ToList();

			//������϶����㲻��
			query = select.RightJoin("TestTypeInfo b on b.Guid = a.TestTypeInfoGuid");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN TestTypeInfo b on b.Guid = a.TestTypeInfoGuid", sql);
			query.ToList();

			query = select.RightJoin("TestTypeInfo b on b.Guid = a.TestTypeInfoGuid and b.Name = ?bname", new { bname = "xxx" });
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a RIGHT JOIN TestTypeInfo b on b.Guid = a.TestTypeInfoGuid and b.Name = ?bname", sql);
			query.ToList();

		}
		[Fact]
		public void Where() {
			//����е�������a.Type��a.Type.Parent ���ǵ�������
			var query = select.Where(a => a.Id == 10);
			var sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.`Id` = 10)", sql);
			query.ToList();

			query = select.Where(a => a.Id == 10 && a.Id > 10 || a.Clicks > 100);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.`Id` = 10 AND a.`Id` > 10 OR a.`Clicks` > 100)", sql);
			query.ToList();

			query = select.Where(a => a.Id == 10).Where(a => a.Clicks > 100);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.`Id` = 10) AND (a.`Clicks` > 100)", sql);
			query.ToList();

			query = select.Where(a => a.Type.Name == "typeTitle");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeInfo` a__Type WHERE (a__Type.`Name` = 'typeTitle')", sql);
			query.ToList();

			query = select.Where(a => a.Type.Name == "typeTitle" && a.Type.Guid == a.TestTypeInfoGuid);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeInfo` a__Type WHERE (a__Type.`Name` = 'typeTitle' AND a__Type.`Guid` = a.`TestTypeInfoGuid`)", sql);
			query.ToList();

			query = select.Where(a => a.Type.Parent.Name == "tparent");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeInfo` a__Type, `TestTypeParentInfo` a__Type__Parent WHERE (a__Type__Parent.`Name` = 'tparent')", sql);
			query.ToList();
			
			//���û�е������ԣ��򵥶������
			query = select.Where<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid && b.Name == "typeTitle");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeInfo` b WHERE (b.`Guid` = a.`TestTypeInfoGuid` AND b.`Name` = 'typeTitle')", sql);
			query.ToList();

			query = select.Where<TestTypeInfo>((a, b) => b.Name == "typeTitle" && b.Guid == a.TestTypeInfoGuid);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeInfo` b WHERE (b.`Name` = 'typeTitle' AND b.`Guid` = a.`TestTypeInfoGuid`)", sql);
			query.ToList();

			query = select.Where<TestTypeInfo, TestTypeParentInfo>((a, b, c) => c.Name == "tparent");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeParentInfo` c WHERE (c.`Name` = 'tparent')", sql);
			query.ToList();

			//����һ�� From ��Ķ������
			var query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				.Where(a => a.Id == 10 && c.Name == "xxx")
				.Where(a => b.ParentId == 20));
			sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeParentInfo` c, `TestTypeInfo` b WHERE (a.`Id` = 10 AND c.`Name` = 'xxx') AND (b.`ParentId` = 20)", sql);
			query2.ToList();

			//������϶����㲻��
			query = select.Where("a.clicks > 100 and a.id = ?id", new { id = 10 });
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.clicks > 100 and a.id = ?id)", sql);
			query.ToList();
		}
		[Fact]
		public void WhereIf() {
			//����е�������a.Type��a.Type.Parent ���ǵ�������
			var query = select.WhereIf(true, a => a.Id == 10);
			var sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.`Id` = 10)", sql);
			query.ToList();

			query = select.WhereIf(true, a => a.Id == 10 && a.Id > 10 || a.Clicks > 100);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.`Id` = 10 AND a.`Id` > 10 OR a.`Clicks` > 100)", sql);
			query.ToList();

			query = select.WhereIf(true, a => a.Id == 10).WhereIf(true, a => a.Clicks > 100);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.`Id` = 10) AND (a.`Clicks` > 100)", sql);
			query.ToList();

			query = select.WhereIf(true, a => a.Type.Name == "typeTitle");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeInfo` a__Type WHERE (a__Type.`Name` = 'typeTitle')", sql);
			query.ToList();

			query = select.WhereIf(true, a => a.Type.Name == "typeTitle" && a.Type.Guid == a.TestTypeInfoGuid);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeInfo` a__Type WHERE (a__Type.`Name` = 'typeTitle' AND a__Type.`Guid` = a.`TestTypeInfoGuid`)", sql);
			query.ToList();

			query = select.WhereIf(true, a => a.Type.Parent.Name == "tparent");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeInfo` a__Type, `TestTypeParentInfo` a__Type__Parent WHERE (a__Type__Parent.`Name` = 'tparent')", sql);
			query.ToList();

			//����һ�� From ��Ķ������
			var query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				.WhereIf(true, a => a.Id == 10 && c.Name == "xxx")
				.WhereIf(true, a => b.ParentId == 20));
			sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a, `TestTypeParentInfo` c, `TestTypeInfo` b WHERE (a.`Id` = 10 AND c.`Name` = 'xxx') AND (b.`ParentId` = 20)", sql);
			query2.ToList();

			//������϶����㲻��
			query = select.WhereIf(true, "a.clicks > 100 and a.id = ?id", new { id = 10 });
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a WHERE (a.clicks > 100 and a.id = ?id)", sql);
			query.ToList();

			// ==========================================WhereIf(false)

			//����е�������a.Type��a.Type.Parent ���ǵ�������
			query = select.WhereIf(false, a => a.Id == 10);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query.ToList();

			query = select.WhereIf(false, a => a.Id == 10 && a.Id > 10 || a.Clicks > 100);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query.ToList();

			query = select.WhereIf(false, a => a.Id == 10).WhereIf(false, a => a.Clicks > 100);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query.ToList();

			query = select.WhereIf(false, a => a.Type.Name == "typeTitle");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query.ToList();

			query = select.WhereIf(false, a => a.Type.Name == "typeTitle" && a.Type.Guid == a.TestTypeInfoGuid);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query.ToList();

			query = select.WhereIf(false, a => a.Type.Parent.Name == "tparent");
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query.ToList();

			//����һ�� From ��Ķ������
			query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				.WhereIf(false, a => a.Id == 10 && c.Name == "xxx")
				.WhereIf(false, a => b.ParentId == 20));
			sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query2.ToList();

			//������϶����㲻��
			query = select.WhereIf(false, "a.clicks > 100 and a.id = ?id", new { id = 10 });
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topic` a", sql);
			query.ToList();
		}
		[Fact]
		public void WhereExists() {
			var sql2222 = select.Where(a => select.Where(b => b.Id == a.Id).Any()).ToList();

			sql2222 = select.Where(a =>
				select.Where(b => b.Id == a.Id && select.Where(c => c.Id == b.Id).Where(d => d.Id == a.Id).Where(e => e.Id == b.Id)

				.Offset(a.Id)

				.Any()
				).Any()
			).ToList();
		}
		[Fact]
		public void GroupBy() {
			var groupby = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				.Where(a => a.Id == 1)
			)
			.GroupBy((a, b, c) => new { tt2 = a.Title.Substring(0, 2), mod4 = a.Id % 4 })
			.Having(a => a.Count() > 0 && a.Avg(a.Key.mod4) > 0 && a.Max(a.Key.mod4) > 0)
			.Having(a => a.Count() < 300 || a.Avg(a.Key.mod4) < 100)
			.OrderBy(a => a.Key.tt2)
			.OrderByDescending(a => a.Count())
			.ToList(a => new {
				a.Key.tt2,
				cou1 = a.Count(),
				arg1 = a.Avg(a.Key.mod4),
				ccc2 = a.Key.tt2 ?? "now()",
				//ccc = Convert.ToDateTime("now()"), partby = Convert.ToDecimal("sum(num) over(PARTITION BY server_id,os,rid,chn order by id desc)")
			});
		}
		[Fact]
		public void ToAggregate() {
			var sql = select.ToAggregate(a => new { sum = a.Sum(a.Key.Id + 11.11), avg = a.Avg(a.Key.Id), count = a.Count(), max = a.Max(a.Key.Id), min = a.Min(a.Key.Id) });
		}

		[Fact]
		public void OrderBy() {
		}
		[Fact]
		public void Skip_Offset() {
		}
		[Fact]
		public void Take_Limit() {
		}
		[Fact]
		public void Page() {
		}
		[Fact]
		public void Sum() {
		}
		[Fact]
		public void Min() {
		}
		[Fact]
		public void Max() {
		}
		[Fact]
		public void Avg() {
		}
		[Fact]
		public void As() {
		}

		[Fact]
		public void AsTable() {
			Func<Type, string, string> tableRule = (type, oldname) => {
				if (type == typeof(Topic)) return oldname + "AsTable1";
				else if (type == typeof(TestTypeInfo)) return oldname + "AsTable2";
				return oldname + "AsTable";
			};

			//����е�������a.Type��a.Type.Parent ���ǵ�������
			var query = select.LeftJoin(a => a.Type.Guid == a.TestTypeInfoGuid).AsTable(tableRule);
			var sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid`", sql);

			query = select.LeftJoin(a => a.Type.Guid == a.TestTypeInfoGuid && a.Type.Name == "xxx").AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid` AND a__Type.`Name` = 'xxx'", sql);

			query = select.LeftJoin(a => a.Type.Guid == a.TestTypeInfoGuid && a.Type.Name == "xxx").Where(a => a.Type.Parent.Id == 10).AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeParentInfoAsTable` a__Type__Parent ON 1 = 1 LEFT JOIN `TestTypeInfoAsTable2` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid` AND a__Type.`Name` = 'xxx' WHERE (a__Type__Parent.`Id` = 10)", sql);

			//���û�е�������
			query = select.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid).AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` b ON b.`Guid` = a.`TestTypeInfoGuid`", sql);

			query = select.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid && b.Name == "xxx").AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` b ON b.`Guid` = a.`TestTypeInfoGuid` AND b.`Name` = 'xxx'", sql);

			query = select.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid && b.Name == "xxx").Where(a => a.Type.Parent.Id == 10).AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeParentInfoAsTable` b__Parent ON 1 = 1 LEFT JOIN `TestTypeInfoAsTable2` b ON b.`Guid` = a.`TestTypeInfoGuid` AND b.`Name` = 'xxx' WHERE (b__Parent.`Id` = 10)", sql);

			//�������
			query = select
				.LeftJoin(a => a.Type.Guid == a.TestTypeInfoGuid)
				.LeftJoin(a => a.Type.Parent.Id == a.Type.ParentId).AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a__Type.`Guid`, a__Type.`ParentId`, a__Type.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` a__Type ON a__Type.`Guid` = a.`TestTypeInfoGuid` LEFT JOIN `TestTypeParentInfoAsTable` a__Type__Parent ON a__Type__Parent.`Id` = a__Type.`ParentId`", sql);

			query = select
				.LeftJoin<TestTypeInfo>((a, b) => b.Guid == a.TestTypeInfoGuid)
				.LeftJoin<TestTypeParentInfo>((a, c) => c.Id == a.Type.ParentId).AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` b ON b.`Guid` = a.`TestTypeInfoGuid` LEFT JOIN `TestTypeParentInfoAsTable` c ON c.`Id` = b.`ParentId`", sql);

			//���û�е�������b��c������ϵ
			var query2 = select.From<TestTypeInfo, TestTypeParentInfo>((s, b, c) => s
				 .LeftJoin(a => a.TestTypeInfoGuid == b.Guid)
				 .LeftJoin(a => b.ParentId == c.Id)).AsTable(tableRule);
			sql = query2.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, b.`Guid`, b.`ParentId`, b.`Name`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN `TestTypeInfoAsTable2` b ON a.`TestTypeInfoGuid` = b.`Guid` LEFT JOIN `TestTypeParentInfoAsTable` c ON b.`ParentId` = c.`Id`", sql);

			//������϶����㲻��
			query = select.LeftJoin("TestTypeInfo b on b.Guid = a.TestTypeInfoGuid").AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN TestTypeInfo b on b.Guid = a.TestTypeInfoGuid", sql);

			query = select.LeftJoin("TestTypeInfo b on b.Guid = a.TestTypeInfoGuid and b.Name = ?bname", new { bname = "xxx" }).AsTable(tableRule);
			sql = query.ToSql().Replace("\r\n", "");
			Assert.Equal("SELECT a.`Id`, a.`Clicks`, a.`TestTypeInfoGuid`, a.`Title`, a.`CreateTime` FROM `tb_topicAsTable1` a LEFT JOIN TestTypeInfo b on b.Guid = a.TestTypeInfoGuid and b.Name = ?bname", sql);
		}
	}
}
